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
            var bounce = new Mock<ITargetBuilderBounce>().Object;

            var buildActions = new StringWriter();

            dependent.Setup(d => d.Dependencies).Returns(new[] {dependency.Object});
            dependent.Setup(d => d.BeforeBuild(bounce)).Callback(() => buildActions.Write("before build dependent;"));
            dependent.Setup(d => d.Build(bounce)).Callback(() => buildActions.Write("build dependent;"));
            dependency.Setup(d => d.BeforeBuild(bounce)).Callback(() => buildActions.Write("before build dependency;"));
            dependency.Setup(d => d.Build(bounce)).Callback(() => buildActions.Write("build dependency;"));

            var builder = new TargetBuilder(bounce);
            builder.Build(dependent.Object);

            Assert.That(buildActions.ToString(), Is.EqualTo(@"before build dependent;before build dependency;build dependency;build dependent;"));
        }

        [Test]
        public void ShouldCleanDependentsBeforeDependencies() {
            var dependent = new Mock<ITask>();
            var dependency = new Mock<ITask>();
            var bounce = new Mock<ITargetBuilderBounce>().Object;

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
            var bounce = new Mock<ITargetBuilderBounce>().Object;

            all.Setup(d => d.Dependencies).Returns(new[] { dependent1.Object, dependent2.Object });
            dependent1.Setup(d => d.Dependencies).Returns(new[] { twiceADependency.Object });
            dependent2.Setup(d => d.Dependencies).Returns(new[] { twiceADependency.Object });

            var builder = new TargetBuilder(bounce);
            builder.Build(all.Object);

            twiceADependency.Verify(t => t.Build(bounce), Times.Once());
        }
    }
}