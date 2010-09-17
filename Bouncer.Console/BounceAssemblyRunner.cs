using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Bouncer.Console {
    class BounceAssemblyRunner {
        public void Run(string [] args) {
            if (args.Length < 1) {
                string exeName = Path.GetFileNameWithoutExtension(GetType().Assembly.Location);
                System.Console.WriteLine("usage: {0} ASSEMBLY", exeName);
                Environment.Exit(1);
            }
            string assemblyFileName = args[0];
            SetCwdToAssemblyDirectory(assemblyFileName);

            if (!File.Exists(assemblyFileName)) {
                System.Console.WriteLine("assembly at: `{0}', does not exist", assemblyFileName);
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

        private void SetCwdToAssemblyDirectory(string assemblyFileName) {
            string assemblyDirectory = Path.GetDirectoryName(assemblyFileName);

            if (!String.IsNullOrEmpty(assemblyDirectory)) {
                Directory.SetCurrentDirectory(assemblyDirectory);
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