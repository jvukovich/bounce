using System;
using System.Collections.Generic;
using System.IO;
using Bounce.Framework;
using Moq;
using NUnit.Framework;

namespace Bounce.Tests
{
    [TestFixture]
    public class BouncerBuilderTest
    {
        [Test]
        public void ShouldBuildIfLastBuiltBeforeDependencyBuilt() {
            var dep1 = new FakeTarget {LastBuilt = new DateTime(2010, 5, 5), Dependencies = new ITarget[0]};
            var dep2 = new FakeTarget {LastBuilt = new DateTime(2010, 5, 3), Dependencies = new ITarget[0]};
            var bouncer = new Mock<FakeTarget>();
            bouncer.Object.Dependencies = new[] {dep1, dep2};
            bouncer.Object.LastBuilt = new DateTime(2010, 5, 4);

            var builder = new TargetBuilder();
            builder.Build(bouncer.Object);

            bouncer.Verify(b => b.Build());
        }

        [Test]
        public void ShouldNotBuildIfLastBuiltAfterDependencyBuilt() {
            var dep1 = new FakeTarget {LastBuilt = new DateTime(2010, 5, 5), Dependencies = new ITarget[0]};
            var dep2 = new FakeTarget {LastBuilt = new DateTime(2010, 5, 4), Dependencies = new ITarget[0]};
            var bouncer = new Mock<FakeTarget>();
            bouncer.Object.Dependencies = new[] {dep1, dep2};
            bouncer.Object.LastBuilt = new DateTime(2010, 5, 6);

            var builder = new TargetBuilder();
            builder.Build(bouncer.Object);

            bouncer.Verify(b => b.Build(), Times.Never());
        }

        [Test]
        public void ShouldBuildIfHasNoDependenciesAndNoLastBuilt() {
            var bouncer = new Mock<FakeTarget>();
            bouncer.Object.Dependencies = new ITarget[0];
            bouncer.Object.LastBuilt = null;

            var builder = new TargetBuilder();
            builder.Build(bouncer.Object);

            bouncer.Verify(b => b.Build());
        }

        [Test]
        public void ShouldNotBuildIfHasNoDependenciesAndLastBuilt() {
            var bouncer = new Mock<FakeTarget>();
            bouncer.Object.Dependencies = new ITarget[0];
            bouncer.Object.LastBuilt = new DateTime(2010, 5, 4);

            var builder = new TargetBuilder();
            builder.Build(bouncer.Object);

            bouncer.Verify(b => b.Build(), Times.Never());
        }

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
