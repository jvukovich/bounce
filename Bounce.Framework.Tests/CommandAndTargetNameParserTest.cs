using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;

namespace Bounce.Framework.Tests {
    [TestFixture]
    public class CommandAndTargetParserTest {
        [Test]
        public void ShouldReturnCommandAndTargetNamesWhenCleanCommandIsProvided() {
            AssertCommandAndTargetsParsed(BounceCommand.Clean, new[] {"clean", "Target1", "Target2"});
        }

        [Test]
        public void ShouldReturnCommandAndTargetNamesWhenBuildCommandIsProvided() {
            AssertCommandAndTargetsParsed(BounceCommand.Build, new[] {"build", "Target1", "Target2"});
        }

        [Test]
        public void ShouldAssumeBuildWhenNoCommandIsProvided() {
            AssertCommandAndTargetsParsed(BounceCommand.Build, new[] {"Target1", "Target2"});
        }

        [Test]
        public void ShouldReturnNoTargetsIfNoneSpecified() {
            var parser = new CommandAndTargetParser();
            var targets = new Dictionary<string, ITask>();
            
            var commandAndTargetNames = parser.ParseCommandAndTargetNames(new [] {"build"}, targets);
            Assert.That(commandAndTargetNames.Command, Is.EqualTo(BounceCommand.Build));
            Assert.That(commandAndTargetNames.Targets.Count(), Is.EqualTo(0));
        }

        [Test]
        public void ShouldExpectTargetIfBuildIsUpperCase() {
            var parser = new CommandAndTargetParser();
            var targets = new Dictionary<string, ITask>();

            Assert.That(() => parser.ParseCommandAndTargetNames(new[] {"Build"}, targets),
                        Throws.InstanceOf(typeof (NoSuchTargetException)));
        }

        [Test]
        public void ShouldThrowIfNoTargetFound() {
            var parser = new CommandAndTargetParser();
            var targets = new Dictionary<string, ITask>();

            Assert.That(() => parser.ParseCommandAndTargetNames(new[] {"build", "NoTarget"}, targets),
                        Throws.InstanceOf(typeof (NoSuchTargetException)));
        }

        [Test]
        public void ShouldReturnNoTargetsIfNoneSpecifiedAndNoCommandSpecified() {
            var parser = new CommandAndTargetParser();
            var targets = new Dictionary<string, ITask>();
            
            var commandAndTargetNames = parser.ParseCommandAndTargetNames(new string [0], targets);
            Assert.That(commandAndTargetNames.Command, Is.EqualTo(BounceCommand.Build));
            Assert.That(commandAndTargetNames.Targets.Count(), Is.EqualTo(0));
        }

        private void AssertCommandAndTargetsParsed(BounceCommand bounceCommand, string[] buildArguments) {
            var parser = new CommandAndTargetParser();
            var targets = new Dictionary<string, ITask>();
            var target1 = new Mock<ITask>().Object;
            targets.Add("Target1", target1);
            var target2 = new Mock<ITask>().Object;
            targets.Add("Target2", target2);

            var commandAndTargetNames = parser.ParseCommandAndTargetNames(buildArguments, targets);

            Assert.That(commandAndTargetNames.Command, Is.EqualTo(bounceCommand));
            Assert.That(commandAndTargetNames.Targets.ElementAt(0).Name, Is.EqualTo("Target1"));
            Assert.That(commandAndTargetNames.Targets.ElementAt(0).Task, Is.SameAs(target1));
            Assert.That(commandAndTargetNames.Targets.ElementAt(1).Name, Is.EqualTo("Target2"));
            Assert.That(commandAndTargetNames.Targets.ElementAt(1).Task, Is.SameAs(target2));
        }
    }
}