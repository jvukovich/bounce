using System.IO;
using Moq;
using NUnit.Framework;

namespace Bounce.Framework.Tests {
    [TestFixture]
    public class TargetBuilderTest {
        [Test]
        public void ShouldBuildDependenciesBeforeDependencts() {
            var dependent = new Mock<ITask>();
            var dependency = new Mock<ITask>();
            ITargetBuilderBounce bounce = GetBounce();

            var buildActions = new StringWriter();

            dependent.Setup(d => d.Dependencies).Returns(new[] {dependency.Object});
            dependent.Setup(d => d.Invoke(BounceCommand.Build, bounce)).Callback(() => buildActions.Write("build dependent;"));
            dependency.Setup(d => d.Invoke(BounceCommand.Build, bounce)).Callback(() => buildActions.Write("build dependency;"));

            var builder = new TargetInvoker(bounce);
            builder.Invoke(BounceCommand.Build, dependent.Object);

            Assert.That(buildActions.ToString(), Is.EqualTo(@"build dependency;build dependent;"));
        }

        [Test]
        public void EachTaskShouldDescribeThemSelvesBeforeBuild() {
            var dependent = new FakeDescribingTask("one");
            var dependency = new FakeDescribingTask("two");
            dependent.Dependencies = new[] {dependency};

            ITargetBuilderBounce bounce = GetBounce();

            var builder = new TargetInvoker(bounce);
            builder.Invoke(BounceCommand.Build, dependent);

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
                .Setup(b => b.TaskScope(It.IsAny<ITask>(), It.IsAny<BounceCommand>(), It.IsAny<string>()))
                .Returns(new Mock<ITaskScope>().Object);
            var descriptionOutput = new StringWriter();
            bounceMock
                .Setup(b => b.DescriptionOutput).Returns(descriptionOutput);
            return bounceMock.Object;
        }

        [Test]
        public void ShouldCleanDependentsBeforeDependencies() {
            var dependent = new Mock<ITask>();
            var dependency = new Mock<ITask>();
            ITargetBuilderBounce bounce = GetBounce();

            var cleanActions = new StringWriter();

            dependent.Setup(d => d.Dependencies).Returns(new[] {dependency.Object});
            dependent.Setup(d => d.Invoke(BounceCommand.Clean, bounce)).Callback(() => cleanActions.Write("clean dependent;"));

            dependency.Setup(d => d.Invoke(BounceCommand.Clean, bounce)).Callback(() => cleanActions.Write("clean dependency;"));

            var builder = new TargetInvoker(bounce);
            builder.Invoke(BounceCommand.Clean, dependent.Object);

            Assert.That(cleanActions.ToString(), Is.EqualTo(@"clean dependent;clean dependency;"));
        }

        [Test]
        public void ShouldOnlyBuildTasksOnceEvenIfTheyAreDependedUponTwice()
        {
            var all = new Mock<ITask>();
            var dependent1 = new Mock<ITask>();
            var dependent2 = new Mock<ITask>();
            var twiceADependency = new Mock<ITask>();
            ITargetBuilderBounce bounce = GetBounce();

            all.Setup(d => d.Dependencies).Returns(new[] { dependent1.Object, dependent2.Object });
            dependent1.Setup(d => d.Dependencies).Returns(new[] { twiceADependency.Object });
            dependent2.Setup(d => d.Dependencies).Returns(new[] { twiceADependency.Object });

            var invoker = new TargetInvoker(bounce);
            invoker.Invoke(BounceCommand.Build, all.Object);

            twiceADependency.Verify(t => t.Invoke(BounceCommand.Build, bounce), Times.Once());
        }
    }
}