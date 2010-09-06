using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Bouncer.Console {
    class BounceAssemblyRunner {
        public void Run(string [] args) {
            if (args.Length < 1) {
                System.Console.WriteLine("usage: bounce ASSEMBLY");
                return;
            }
            string assemblyPath = args[0];
            Directory.SetCurrentDirectory(Path.GetDirectoryName(assemblyPath));

            Assembly a = Assembly.LoadFrom(assemblyPath);

            BounceAssemblyAndTargetsProperty bounceAssemblyAndTargetsProperty = FindTargetsMember(a);

            Assembly bounceAssembly = bounceAssemblyAndTargetsProperty.BounceAssembly;
            Type runnerType = bounceAssembly.GetType("Bounce.BounceRunner");
            ConstructorInfo runnerConstructor = runnerType.GetConstructor(new Type[0]);
            object runner = runnerConstructor.Invoke(new object[0]);
            object parameters = runnerType.GetProperty("Parameters").GetValue(runner, new object[0]);

            object targets = bounceAssemblyAndTargetsProperty.TargetsProperty.Invoke(null, new[] { parameters });
            runnerType.GetMethod("Run").Invoke(runner, new[] { args, targets });
        }

        BounceAssemblyAndTargetsProperty FindTargetsMember(Assembly assembly) {
            var allProperties = assembly.GetTypes().SelectMany(t => t.GetMethods(BindingFlags.Static | BindingFlags.Public));

            foreach (var prop in allProperties) {
                foreach (var attr in prop.GetCustomAttributes(false)) {
                    var attrType = attr.GetType();
                    if (attrType.FullName == "Bounce.TargetsAttribute") {
                        return new BounceAssemblyAndTargetsProperty { BounceAssembly = attrType.Assembly, TargetsProperty = prop };
                    }
                }
            }

            return null;
        }

        class BounceAssemblyAndTargetsProperty {
            public MethodInfo TargetsProperty;
            public Assembly BounceAssembly;
        }
    }
}