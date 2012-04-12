using System;
using System.Collections;
using System.Linq;
using System.Reflection;

namespace Bounce.Console {
    
    [Serializable]
    class BounceAssemblyRunner {
        private readonly BeforeBounceScriptRunner BeforeBounceScriptRunner;
        private string assemblyFileName;
        private string[] arguments;

        public BounceAssemblyRunner() {
            BeforeBounceScriptRunner = new BeforeBounceScriptRunner();
        }

        public void Run(string[] args) {
            try {
                FindTargetsAssemblyAndRunBounce(args);
            } catch (BounceConsoleException bce) {
                bce.Explain(System.Console.Error);
                Environment.Exit(1);
            } catch (Exception e) {
                System.Console.Error.WriteLine(e);
                Environment.Exit(1);
            }
        }

        private void FindTargetsAssemblyAndRunBounce(string[] args) {
            var optionsAndArguments = GetAssemblyFileName(args);

            BeforeBounceScriptRunner.RunBeforeBounceScript(optionsAndArguments);

            assemblyFileName = optionsAndArguments.TargetsAssembly.Executable;
            arguments = optionsAndArguments.RemainingArguments;

            var appDomainSetup = new AppDomainSetup { ShadowCopyFiles = true.ToString() };
            var appDomain = AppDomain.CreateDomain("Bounce", null, appDomainSetup);

            try
            {
                //call back to transfer control to other app domain
                appDomain.DoCallBack(AppDomainLoaderCallBack);
            }
            finally
            {
                AppDomain.Unload(appDomain);
            }
        }

        private void AppDomainLoaderCallBack()
        {
            System.Console.WriteLine("- Loading bounce targets assembly (with shadow-copy enabled) from '" + assemblyFileName + "'.");
            var assembly = Assembly.LoadFrom(assemblyFileName);
            System.Console.WriteLine("- Targets assembly loaded from '" + assembly.Location + "'.");
            var bounceAssemblyAndTargetsProperty = GetTargetsMemberFromAssembly(assembly);

            RunAssembly(bounceAssemblyAndTargetsProperty, arguments);
        }

        private OptionsAndArguments GetAssemblyFileName(string[] args) {
            return new TargetsAssemblyArgumentsParser().GetTargetsAssembly(args);
        }

        private void RunAssembly(BounceAssemblyAndTargetsProperty bounceAssemblyAndTargetsProperty, IEnumerable args) {
            if (args == null) throw new ArgumentNullException("args");
            Assembly bounceAssembly = bounceAssemblyAndTargetsProperty.BounceAssembly;
            Type runnerType = bounceAssembly.GetType("Bounce.Framework.Obsolete.BounceRunner");
            object runner = runnerType.GetConstructor(new Type[0]).Invoke(new object[0]);

            runnerType.GetMethod("Run").Invoke(runner, new object[] { args, bounceAssemblyAndTargetsProperty.GetTargetsMethod });
        }

        BounceAssemblyAndTargetsProperty GetTargetsMemberFromAssembly(Assembly assembly) {
            var allProperties = assembly.GetTypes().SelectMany(t => t.GetMethods(BindingFlags.Static | BindingFlags.Public));

            foreach (var prop in allProperties) {
                foreach (var attr in prop.GetCustomAttributes(false)) {
                    var attrType = attr.GetType();
                    if (attrType.FullName == "Bounce.Framework.Obsolete.TargetsAttribute") {
                        return new BounceAssemblyAndTargetsProperty { BounceAssembly = attrType.Assembly, GetTargetsMethod = prop };
                    }
                }
            }

            throw new TargetsAttributeNotFoundException();
        }

        class BounceAssemblyAndTargetsProperty {
            public MethodInfo GetTargetsMethod;
            public Assembly BounceAssembly;
        }
    }
}