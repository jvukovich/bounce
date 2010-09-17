using System;
using System.IO;
using Bounce.Framework;
using Moq;
using NUnit.Framework;

namespace Bounce.Tests
{
    [TestFixture]
    public class TargetBuilderTest {
        [Test]
        public void ShouldBuildDependenciesBeforeDependencts() {
            var dependent = new Mock<ITarget>();
            var dependency = new Mock<ITarget>();

            var cleanActions = new StringWriter();

            dependent.Setup(d => d.Dependencies).Returns(new[] {dependency.Object});
            dependent.Setup(d => d.Build()).Callback(() => cleanActions.Write("build dependent;"));

            dependency.Setup(d => d.Build()).Callback(() => cleanActions.Write("build dependency;"));

            var builder = new TargetBuilder();
            builder.Build(dependent.Object);

            Assert.That(cleanActions.ToString(), Is.EqualTo(@"build dependency;build dependent;"));
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
