using System.Collections.Generic;
using System.IO;
using Bounce.Framework.Obsolete;
using Moq;
using NUnit.Framework;

namespace Bounce.Framework.Tests.Obsolete {
    [TestFixture]
    public class TargetInvokerTest {
        [Test]
        public void ShouldBuildDependenciesBeforeDependencts() {
            var dependent = new Mock<IObsoleteTask>();
            var dependency = new Mock<IObsoleteTask>();
            ITargetBuilderBounce bounce = GetBounce();

            var buildActions = new StringWriter();

            dependent.Setup(d => d.Dependencies).Returns(new[] {new TaskDependency (dependency.Object)});
            var bounceCommand = new BounceCommandParser().Build;
            dependent.Setup(d => d.Invoke(bounceCommand, bounce)).Callback(() => buildActions.Write("build dependent;"));
            dependency.Setup(d => d.Invoke(bounceCommand, bounce)).Callback(() => buildActions.Write("build dependency;"));

            var builder = new TargetInvoker(bounce);
            builder.Invoke(bounceCommand, dependent.Object);

            Assert.That(buildActions.ToString(), Is.EqualTo(@"build dependency;build dependent;"));
        }

        [Test]
        public void EachTaskShouldDescribeThemSelvesBeforeBuild() {
            var dependent = new FakeDescribingTask("one");
            var dependency = new FakeDescribingTask("two");
            dependent.Dependencies = new[] {new TaskDependency(dependency)};

            ITargetBuilderBounce bounce = GetBounce();

            var builder = new TargetInvoker(bounce);
            builder.Invoke(new BounceCommandParser().Build, dependent);

            Assert.That(bounce.DescriptionOutput.ToString(), Is.EqualTo(@"two;one;"));
        }

        public class FakeDescribingTask : FakeTask {
            private string Description;

            public FakeDescribingTask(string description) {
                Description = description;
            }

            public override void Describe(TextWriter output) {
                output.Write(Description + ";");
            }
        }

        private ITargetBuilderBounce GetBounce() {
            var bounceMock = new Mock<ITargetBuilderBounce>();
            bounceMock
                .Setup(b => b.TaskScope(It.IsAny<IObsoleteTask>(), It.IsAny<IBounceCommand>(), It.IsAny<string>()))
                .Returns(new Mock<ITaskScope>().Object);
            var descriptionOutput = new StringWriter();
            bounceMock
                .Setup(b => b.DescriptionOutput).Returns(descriptionOutput);
            return bounceMock.Object;
        }

        [Test]
        public void ShouldCleanDependentsBeforeDependencies() {
            var dependent = new Mock<IObsoleteTask>();
            var dependency = new Mock<IObsoleteTask>();
            ITargetBuilderBounce bounce = GetBounce();

            var cleanActions = new StringWriter();

            dependent.Setup(d => d.Dependencies).Returns(new[] {new TaskDependency(dependency.Object)});
            var bounceCommand = new BounceCommandParser().Clean;
            dependent.Setup(d => d.Invoke(bounceCommand, bounce)).Callback(() => cleanActions.Write("clean dependent;"));

            dependency.Setup(d => d.Invoke(bounceCommand, bounce)).Callback(() => cleanActions.Write("clean dependency;"));

            var builder = new TargetInvoker(bounce);
            builder.Invoke(bounceCommand, dependent.Object);

            Assert.That(cleanActions.ToString(), Is.EqualTo(@"clean dependent;clean dependency;"));
        }

        [Test]
        public void ShouldOnlyBuildTasksOnceEvenIfTheyAreDependedUponTwice()
        {
            var all = new Mock<IObsoleteTask>();
            var dependent1 = new Mock<IObsoleteTask>();
            var dependent2 = new Mock<IObsoleteTask>();
            var twiceADependency = new Mock<IObsoleteTask>();
            ITargetBuilderBounce bounce = GetBounce();

            all.Setup(d => d.Dependencies).Returns(new[] {new TaskDependency(dependent1.Object), new TaskDependency(dependent2.Object) });
            dependent1.Setup(d => d.Dependencies).Returns(new[] {new TaskDependency(twiceADependency.Object) });
            dependent2.Setup(d => d.Dependencies).Returns(new[] {new TaskDependency(twiceADependency.Object) });

            var invoker = new TargetInvoker(bounce);
            var bounceCommand = new BounceCommandParser().Build;
            invoker.Invoke(bounceCommand, all.Object);

            twiceADependency.Verify(t => t.Invoke(bounceCommand, bounce), Times.Once());
        }

        [Test]
        public void ShouldOnlyCleanTasksOnceEvenIfTheyAreDependedUponTwice()
        {
            var all = new Mock<IObsoleteTask>();
            var dependent1 = new Mock<IObsoleteTask>();
            var dependent2 = new Mock<IObsoleteTask>();
            var twiceADependency = new Mock<IObsoleteTask>();
            ITargetBuilderBounce bounce = GetBounce();

            all.Setup(d => d.Dependencies).Returns(new[] {new TaskDependency(dependent1.Object), new TaskDependency (dependent2.Object) });
            dependent1.Setup(d => d.Dependencies).Returns(new[] {new TaskDependency (twiceADependency.Object) });
            dependent2.Setup(d => d.Dependencies).Returns(new[] {new TaskDependency (twiceADependency.Object) });

            var invoker = new TargetInvoker(bounce);
            var bounceCommand = new BounceCommandParser().Clean;
            invoker.Invoke(bounceCommand, all.Object);

            twiceADependency.Verify(t => t.Invoke(bounceCommand, bounce), Times.Once());
        }

        [Test]
        public void ShouldCleanDependenciesAndTheirDependenciesAfterBuildIfMarkedSo() {
            var artefacts = new HashSet<string>();

            var d = new FakeArtefactTaskWithDependencies(artefacts, "d");
            var a = new FakeArtefactTaskWithDependencies(artefacts, "a");
            var b = new FakeArtefactTaskWithDependencies(artefacts, "b", new[] {new TaskDependency(d)});
            var cDeps = new[] {new TaskDependency(a), new TaskDependency(b) {CleanAfterBuild = true}};
            var c = new FakeArtefactTaskWithDependencies(artefacts, "c", cDeps);

            ITargetBuilderBounce bounce = GetBounce();
            
            var invoker = new TargetInvoker(bounce);
            var bounceCommand = new BounceCommandParser().BuildAndClean;
            invoker.Invoke(bounceCommand, c);
            invoker.CleanAfterBuild(bounceCommand);

            Assert.That(artefacts, Has.Member("a"));
            Assert.That(artefacts, Has.No.Member("b"));
            Assert.That(b.Invoked);
            Assert.That(artefacts, Has.Member("c"));
            Assert.That(artefacts, Has.No.Member("d"));
            Assert.That(d.Invoked);
        }

        [Test]
        public void ShouldNotCleanDepsIfMarkedCleanAfterBuildButAlsoDependedUponElsewhere() {
            var artefacts = new HashSet<string>();

            var b = new FakeArtefactTaskWithDependencies(artefacts, "b");
            var cDeps = new[] {new TaskDependency(b), new TaskDependency (b) {CleanAfterBuild = true}};
            var c = new FakeArtefactTaskWithDependencies(artefacts, "c", cDeps);

            ITargetBuilderBounce bounce = GetBounce();
            
            var invoker = new TargetInvoker(bounce);
            invoker.Invoke(new BounceCommandParser().Build, c);

            Assert.That(artefacts, Has.Member("b"));
            Assert.That(artefacts, Has.Member("c"));
        }

        class FakeArtefactTaskWithDependencies : FakeArtefactTask {
            private readonly IEnumerable<TaskDependency> AdditionalDependencies;
            public bool Invoked;

            public FakeArtefactTaskWithDependencies(HashSet<string> builtArtefacts, string artefactName) : this(builtArtefacts, artefactName, new TaskDependency[0]) {}

            public FakeArtefactTaskWithDependencies(HashSet<string> builtArtefacts, string artefactName, IEnumerable<TaskDependency> additionalDependencies) : base(builtArtefacts, artefactName) {
                AdditionalDependencies = additionalDependencies;
            }

            protected override IEnumerable<TaskDependency> RegisterAdditionalDependencies() {
                return AdditionalDependencies;
            }

            public override void Invoke(IBounceCommand command, IBounce bounce)
            {
                base.Invoke(command, bounce);
                Invoked = true;
            }
        }
    }
}