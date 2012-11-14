using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Bounce.Console {
    
    [Serializable]
    class BounceAssemblyRunner {
        private readonly BeforeBounceScriptRunner BeforeBounceScriptRunner;
        private string bounceDirectory;
        private string[] arguments;
        private int ExitCode = 1;

        public BounceAssemblyRunner() {
            BeforeBounceScriptRunner = new BeforeBounceScriptRunner();
        }

        public int Run(string[] args) {
            try {
                return FindTargetsAssemblyAndRunBounce(args);
            } catch (BounceConsoleException bce) {
                bce.Explain(System.Console.Error);
                return 1;
            } catch (Exception e) {
                System.Console.Error.WriteLine(e);
                return 1;
            }
        }

        private int FindTargetsAssemblyAndRunBounce(string[] args) {
            var optionsAndArguments = GetAssemblyFileName(args);

            BeforeBounceScriptRunner.RunBeforeBounceScript(optionsAndArguments);

            bounceDirectory = optionsAndArguments.BounceDirectory;
            arguments = optionsAndArguments.RemainingArguments;

            var appDomainSetup = new AppDomainSetup { ShadowCopyFiles = "true" };
            var appDomain = AppDomain.CreateDomain("Bounce", null, appDomainSetup);

            try {
                //call back to transfer control to other app domain
                appDomain.DoCallBack(RunTask);
                return 0;
            } catch (Exception) {
                return 1;
            } finally
            {
                AppDomain.Unload(appDomain);
            }
        }

        private void RunTask() {
            var bounceAssembly = ReferencedBounceAssembly();

            RunBounce(bounceAssembly);
        }

        private void RunBounce(Assembly bounceAssembly) {
            Type runnerType = bounceAssembly.GetType("Bounce.Framework.BounceRunner");
            object runner = runnerType.GetConstructor(new Type[0]).Invoke(new object[0]);
            var exitCode = (int) runnerType.GetMethod("Run").Invoke(runner, new object[] {bounceDirectory, arguments});
            if (exitCode != 0) {
                throw new BadExitException();
            }
        }

        private Assembly ReferencedBounceAssembly() {
            var bounceAssembly = FindBounceAssembly();
            if (bounceAssembly != null) {
                return bounceAssembly;
            }

            bounceAssembly = FindBounceAssemblyReferencedFromBounceDirectory();
            if (bounceAssembly != null) {
                return bounceAssembly;
            }

            throw new TasksNotFoundException();
        }

        private Assembly FindBounceAssemblyReferencedFromBounceDirectory() {
            foreach (var file in Directory.GetFiles(bounceDirectory)) {
                if (IsExecutable(file)) {
                    var assembly = Assembly.LoadFrom(file);
                    var allMethods =
                        assembly.GetTypes().SelectMany(t => t.GetMethods(BindingFlags.Public | BindingFlags.Instance));

                    foreach (var method in allMethods) {
                        foreach (var attr in method.GetCustomAttributes(false)) {
                            var attrType = attr.GetType();
                            if (attrType.FullName == "Bounce.Framework.TaskAttribute") {
                                return attrType.Assembly;
                            }
                        }
                    }
                }
            }

            return null;
        }

        private Assembly FindBounceAssembly() {
            var path = Path.Combine(bounceDirectory, "Bounce.Framework.dll");
            if (File.Exists(path)) {
                return Assembly.LoadFrom(path);
            } else {
                return null;
            }
        }

        private static bool IsExecutable(string file) {
            return file.EndsWith(".exe", StringComparison.InvariantCultureIgnoreCase)
                || file.EndsWith(".dll", StringComparison.InvariantCultureIgnoreCase);
        }

        private OptionsAndArguments GetAssemblyFileName(string[] args) {
            return new TargetsAssemblyArgumentsParser().GetTargetsAssembly(args);
        }
    }
}