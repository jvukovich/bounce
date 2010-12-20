using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;

namespace Bounce.Framework.Tests.Features {
    [TestFixture]
    public class CleanAfterBuildFeature {
        private static HashSet<string> BuiltArtefacts;

        [SetUp]
        public void SetUp() {
            BuiltArtefacts = new HashSet<string>();
        }

        [Test]
        public void ShouldCleanTasksAfterBuildIfTheyreMarkedCleanAfterBuild() {
            MethodInfo method = typeof (Targets).GetMethod("GetTargets");
            new BounceRunner().Run(new[] {"build", "C"}, method);

            Assert.That(BuiltArtefacts, Has.Member("a"));
            Assert.That(BuiltArtefacts, Has.No.Member("b"));
            Assert.That(BuiltArtefacts, Has.Member("c"));
        }

        [Test]
        public void ShouldNotCleanIfBuildAndKeep() {
            MethodInfo method = typeof (Targets).GetMethod("GetTargets");
            new BounceRunner().Run(new[] {"buildandkeep", "C"}, method);

            Assert.That(BuiltArtefacts, Has.Member("a"));
            Assert.That(BuiltArtefacts, Has.Member("b"));
            Assert.That(BuiltArtefacts, Has.Member("c"));
        }

        [Test]
        public void ShouldNotCleanTaskIfRequiredForDifferentTarget() {
            MethodInfo method = typeof (Targets).GetMethod("GetTargets");
            new BounceRunner().Run(new[] {"build", "C", "DependsOnTaskToBeCleaned"}, method);

            Assert.That(BuiltArtefacts, Has.Member("a"));
            Assert.That(BuiltArtefacts, Has.Member("b"));
            Assert.That(BuiltArtefacts, Has.Member("c"));
        }

        class Targets {
            public static object GetTargets() {
                var notToBeCleanedBecauseTarget = new TaskToBeCleaned(BuiltArtefacts, "b");
                var taskWithDeps = new TaskWithDeps(BuiltArtefacts, "c") {
                                                                             NotToBeCleaned = new FakeArtefactTask(BuiltArtefacts, "a"),
                                                                             ToBeCleaned = notToBeCleanedBecauseTarget
                                                                         };

                return new {
                    C = taskWithDeps,
                    DependsOnTaskToBeCleaned = notToBeCleanedBecauseTarget,
                };
            }
        }

        class TaskToBeCleaned : FakeArtefactTask {
            public TaskToBeCleaned(HashSet<string> builtArtefacts, string artefactName) : base(builtArtefacts, artefactName) {}
        }

        class TaskWithDeps : FakeArtefactTask {
            [Dependency] public ITask NotToBeCleaned;
            [Dependency, CleanAfterBuild] public ITask ToBeCleaned;

            public TaskWithDeps(HashSet<string> builtArtefacts, string artefactName) : base(builtArtefacts, artefactName) {}
        }
    }
}