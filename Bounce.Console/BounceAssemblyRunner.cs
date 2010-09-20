using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Bounce.Console {
    class BounceAssemblyRunner {
        public void Run(string [] args) {
            string assemblyFileName = FindTargetsAssembly(Directory.GetCurrentDirectory());

            if (assemblyFileName == null) {
                System.Console.WriteLine(@"unable to find Bounce\Targets.dll assembly in this or any parent directory");
                Environment.Exit(1);
            }

            Assembly a = Assembly.LoadFrom(assemblyFileName);

            BounceAssemblyAndTargetsProperty bounceAssemblyAndTargetsProperty = FindTargetsMember(a);

            if (bounceAssemblyAndTargetsProperty == null)
            {
                System.Console.WriteLine("assembly contains no [Targets] method. Try adding something like this:");
                System.Console.WriteLine();
                System.Console.WriteLine(
                    @"public class BuildTargets {
    [Bounce.Framework.Targets]
    public static object Targets (IParameters parameters) {
        return new {
            MyTarget = ...
        };
    }
}
");
                Environment.Exit(1);
            }
            else {
                RunAssembly(bounceAssemblyAndTargetsProperty, args);
            }
        }

        private string FindTargetsAssembly(string currentDir) {
            if (String.IsNullOrEmpty(currentDir)) {
                return null;
            }

            var targetsDll = Path.Combine(Path.Combine(currentDir, "Bounce"), "Targets.dll");
            if (File.Exists(targetsDll)) {
                return targetsDll;
            } else {
                return FindTargetsAssembly(Path.GetDirectoryName(currentDir));
            }
        }

        private void RunAssembly(BounceAssemblyAndTargetsProperty bounceAssemblyAndTargetsProperty, string[] args) {
            Assembly bounceAssembly = bounceAssemblyAndTargetsProperty.BounceAssembly;
            Type runnerType = bounceAssembly.GetType("Bounce.Framework.BounceRunner");
            object runner = runnerType.GetConstructor(new Type[0]).Invoke(new object[0]);

            runnerType.GetMethod("Run").Invoke(runner, new object[] { args, bounceAssemblyAndTargetsProperty.GetTargetsMethod });
        }

        BounceAssemblyAndTargetsProperty FindTargetsMember(Assembly assembly) {
            var allProperties = assembly.GetTypes().SelectMany(t => t.GetMethods(BindingFlags.Static | BindingFlags.Public));

            foreach (var prop in allProperties) {
                foreach (var attr in prop.GetCustomAttributes(false)) {
                    var attrType = attr.GetType();
                    if (attrType.FullName == "Bounce.Framework.TargetsAttribute") {
                        return new BounceAssemblyAndTargetsProperty { BounceAssembly = attrType.Assembly, GetTargetsMethod = prop };
                    }
                }
            }

            return null;
        }

        class BounceAssemblyAndTargetsProperty {
            public MethodInfo GetTargetsMethod;
            public Assembly BounceAssembly;
        }
    }
}