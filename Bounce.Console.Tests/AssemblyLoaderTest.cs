using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using NUnit.Framework;

namespace Bounce.Console.Tests {
    [TestFixture]
    public class AssemblyLoaderTest {
        [Test]
        public void ShouldLoadAssemblyByPath() {
            var assemblyPath = @"C:\stuff\bounce\TestBounceAssembly\bin\Debug\TestBounceAssembly.dll";

            Directory.SetCurrentDirectory(Path.GetDirectoryName(assemblyPath));

            Assembly a = Assembly.LoadFrom(assemblyPath);
            Assert.That(a, Is.Not.Null);

            BounceAssemblyAndTargetsProperty bounceAssemblyAndTargetsProperty = FindTargetsMember(a);
            Assert.That(bounceAssemblyAndTargetsProperty, Is.Not.Null);

            Assembly bounceAssembly = bounceAssemblyAndTargetsProperty.BounceAssembly;
            Type runnerType = bounceAssembly.GetType("Bounce.BounceRunner");
            ConstructorInfo runnerConstructor = runnerType.GetConstructor(new Type[0]);
            object runner = runnerConstructor.Invoke(new object[0]);
            object parameters = runnerType.GetProperty("Parameters").GetValue(runner, new object[0]);

            object targets = bounceAssemblyAndTargetsProperty.TargetsProperty.Invoke(null, new[] {parameters});
            runnerType.GetMethod("Run").Invoke(runner, new[] {new string[0], targets});


//            var assemblies = a.GetReferencedAssemblies();
//            foreach (var refA in assemblies) {
//                System.Console.WriteLine(refA);
//            }
//            AssemblyName bounceFrameworkName = assemblies.First(x => x.Name == "Bounce.Framework");
//            System.Console.WriteLine(bounceFrameworkName.FullName);
//            Assembly bounceFramework = Assembly.LoadFrom(bounceFrameworkName.Name + ".dll");
//            Assert.That(bounceFramework, Is.Not.Null);

//            Type runner = a.GetType("Bounce.BounceRunner");
//            Assert.That(runner, Is.Not.Null);
        }

        BounceAssemblyAndTargetsProperty FindTargetsMember(Assembly assembly) {
            var allProperties = assembly.GetTypes().SelectMany(t => t.GetMethods(BindingFlags.Static | BindingFlags.Public));

            foreach (var prop in allProperties) {
                foreach (var attr in prop.GetCustomAttributes(false)) {
                    var attrType = attr.GetType();
                    if (attrType.FullName == "Bounce.TargetsAttribute") {
                        return new BounceAssemblyAndTargetsProperty {BounceAssembly = attrType.Assembly, TargetsProperty = prop};
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
