using System.IO;
using Bounce.Framework;
using Moq;
using NUnit.Framework;

namespace Bounce.Tests {
    [TestFixture]
    public class TargetBuilderTest {
        [Test]
        public void ShouldBuildDependenciesBeforeDependencts() {
            var dependent = new Mock<ITarget>();
            var dependency = new Mock<ITarget>();

            var buildActions = new StringWriter();

            dependent.Setup(d => d.Dependencies).Returns(new[] {dependency.Object});
            dependent.Setup(d => d.BeforeBuild()).Callback(() => buildActions.Write("before build dependent;"));
            dependent.Setup(d => d.Build()).Callback(() => buildActions.Write("build dependent;"));
            dependency.Setup(d => d.BeforeBuild()).Callback(() => buildActions.Write("before build dependency;"));
            dependency.Setup(d => d.Build()).Callback(() => buildActions.Write("build dependency;"));

            var builder = new TargetBuilder();
            builder.Build(dependent.Object);

            Assert.That(buildActions.ToString(), Is.EqualTo(@"before build dependent;before build dependency;build dependency;build dependent;"));
        }

        [Test]
        public void ShouldCleanDependentsBeforeDependencies() {
            var dependent = new Mock<ITarget>();
            var dependency = new Mock<ITarget>();

            var cleanActions = new StringWriter();

            dependent.Setup(d => d.Dependencies).Returns(new[] {dependency.Object});
            dependent.Setup(d => d.Clean()).Callback(() => cleanActions.Write("clean dependent;"));

            dependency.Setup(d => d.Clean()).Callback(() => cleanActions.Write("clean dependency;"));

            var builder = new TargetBuilder();
            builder.Clean(dependent.Object);

            Assert.That(cleanActions.ToString(), Is.EqualTo(@"clean dependent;clean dependency;"));
        }
    }
}