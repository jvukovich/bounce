using System;
using System.Linq;
using System.Reflection;

namespace Bounce.Console {
    class BounceAssemblyRunner {
        private readonly BounceDirectoryCopier BounceDirectoryCopier;
        private readonly BeforeBounceScriptRunner BeforeBounceScriptRunner;

        public BounceAssemblyRunner() {
            BounceDirectoryCopier = new BounceDirectoryCopier();
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
            OptionsAndArguments optionsAndArguments = GetAssemblyFileName(args);

            BeforeBounceScriptRunner.RunBeforeBounceScript(optionsAndArguments);

            string assemblyFileName = BounceDirectoryCopier.CopyBounceDirectory(optionsAndArguments);
            args = optionsAndArguments.RemainingArguments;

            Assembly a = Assembly.LoadFrom(assemblyFileName);
            BounceAssemblyAndTargetsProperty bounceAssemblyAndTargetsProperty = GetTargetsMemberFromAssembly(a);

            RunAssembly(bounceAssemblyAndTargetsProperty, args);
        }

        private OptionsAndArguments GetAssemblyFileName(string[] args) {
            return new TargetsAssemblyArgumentsParser().GetTargetsAssembly(args);
        }

        private void RunAssembly(BounceAssemblyAndTargetsProperty bounceAssemblyAndTargetsProperty, string[] args) {
            Assembly bounceAssembly = bounceAssemblyAndTargetsProperty.BounceAssembly;
            Type runnerType = bounceAssembly.GetType("Bounce.Framework.BounceRunner");
            object runner = runnerType.GetConstructor(new Type[0]).Invoke(new object[0]);

            runnerType.GetMethod("Run").Invoke(runner, new object[] { args, bounceAssemblyAndTargetsProperty.GetTargetsMethod });
        }

        BounceAssemblyAndTargetsProperty GetTargetsMemberFromAssembly(Assembly assembly) {
            var allProperties = assembly.GetTypes().SelectMany(t => t.GetMethods(BindingFlags.Static | BindingFlags.Public));

            foreach (var prop in allProperties) {
                foreach (var attr in prop.GetCustomAttributes(false)) {
                    var attrType = attr.GetType();
                    if (attrType.FullName == "Bounce.Framework.TargetsAttribute") {
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