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
            dependent.Setup(d => d.Build(bounce)).Callback(() => buildActions.Write("build dependent;"));
            dependency.Setup(d => d.Build(bounce)).Callback(() => buildActions.Write("build dependency;"));

            var builder = new TargetBuilder(bounce);
            builder.Build(dependent.Object);

            Assert.That(buildActions.ToString(), Is.EqualTo(@"build dependency;build dependent;"));
        }

        private ITargetBuilderBounce GetBounce() {
            var bounceMock = new Mock<ITargetBuilderBounce>();
            bounceMock
                .Setup(b => b.TaskScope(It.IsAny<ITask>(), It.IsAny<BounceCommand>(), It.IsAny<string>()))
                .Returns(new Mock<ITaskScope>().Object);
            return bounceMock.Object;
        }

        [Test]
        public void ShouldCleanDependentsBeforeDependencies() {
            var dependent = new Mock<ITask>();
            var dependency = new Mock<ITask>();
            ITargetBuilderBounce bounce = GetBounce();

            var cleanActions = new StringWriter();

            dependent.Setup(d => d.Dependencies).Returns(new[] {dependency.Object});
            dependent.Setup(d => d.Clean(bounce)).Callback(() => cleanActions.Write("clean dependent;"));

            dependency.Setup(d => d.Clean(bounce)).Callback(() => cleanActions.Write("clean dependency;"));

            var builder = new TargetBuilder(bounce);
            builder.Clean(dependent.Object);

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

            var builder = new TargetBuilder(bounce);
            builder.Build(all.Object);

            twiceADependency.Verify(t => t.Build(bounce), Times.Once());
        }
    }
}