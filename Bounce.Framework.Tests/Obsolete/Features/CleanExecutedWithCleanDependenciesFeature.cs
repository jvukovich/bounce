using System;
using System.Collections.Generic;
using System.Reflection;
using Bounce.Framework.Obsolete;
using NUnit.Framework;

namespace Bounce.Framework.Tests.Obsolete.Features {
    [TestFixture]
    public class CleanExecutedWithCleanDependenciesFeature {
        private static HashSet<IObsoleteTask> BuiltTasks;

        [SetUp]
        public void SetUp() {
            BuiltTasks = new HashSet<IObsoleteTask>();
        }

        [Test]
        public void Stuff() {
            MethodInfo method = typeof(Targets).GetMethod("GetTargets");
            new ObsoleteBounceRunner().Run(new[] { "build", "A", "D" }, method);
        }

        class Targets {
            public static object GetTargets() {
                var a = new LeafTask("a");
                var b = new LeafTask("b");
                return new {
                               A = new ABTask("A") {A = new LeafTask("a1"), B = new ABTask("AB") {A = a}},
                               D = new ABTask("D") {A = new LeafTask("a2"), B = new ABTask("AB") {A = a}},
                           };
            }
        }

        class AssertsDependenciesBuiltForClean : Task {
            private readonly string Name;

            public AssertsDependenciesBuiltForClean(string name) {
                Name = name;
            }

            public override void Describe(System.IO.TextWriter output) {
                output.Write(Name);
            }

            public override void Build() {
                BuiltTasks.Add(this);
                Console.WriteLine("built {0}", Name);
            }

            public override void Clean() {
                AssertDependenciesBuilt(Dependencies);
                BuiltTasks.Remove(this);
                Console.WriteLine("cleaned {0}", Name);
            }

            private void AssertDependenciesBuilt(IEnumerable<TaskDependency> deps) {
                foreach (var dep in deps) {
                    if (dep != null) {
                        Assert.That(BuiltTasks.Contains(dep.Task), string.Format("{0}[{1}]->{2} should be built for task to be cleaned", Name, dep.Name, ((AssertsDependenciesBuiltForClean) dep.Task).Name));
                    }
                }
            }
        }

        class LeafTask : AssertsDependenciesBuiltForClean {
            public LeafTask(string name) : base(name) {}
        }

        class ABTask : AssertsDependenciesBuiltForClean {
            [Dependency] public IObsoleteTask A;
            [Dependency, CleanAfterBuild] public IObsoleteTask B;

            public ABTask(string name) : base(name) {}
        }
    }
}