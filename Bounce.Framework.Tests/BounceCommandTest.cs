using NUnit.Framework;

namespace Bounce.Framework.Tests {
    [TestFixture]
    public class BounceCommandTest {
        [Test]
        public void BuildCommandTest() {
            IBounceCommand buildCommand = new BounceCommandParser().Parse("build");
            Assert.That(buildCommand.CommandLineCommand, Is.EqualTo("build"));
            AssertBuildCommandProperties(buildCommand);

            var cleanAfterBuildCommand = buildCommand.CleanAfterBuildCommand;
            Assert.That(cleanAfterBuildCommand, Is.Not.Null);
            AssertCleanCommandProperties(cleanAfterBuildCommand);
        }

        [Test]
        public void BuildAndKeepCommandTest() {
            IBounceCommand buildCommand = new BounceCommandParser().Parse("buildandkeep");
            Assert.That(buildCommand.CommandLineCommand, Is.EqualTo("buildandkeep"));
            AssertBuildCommandProperties(buildCommand);

            var cleanAfterBuildCommand = buildCommand.CleanAfterBuildCommand;
            Assert.That(cleanAfterBuildCommand, Is.Null);
        }

        private void AssertBuildCommandProperties(IBounceCommand command) {
            Assert.That(command.PastTense, Is.EqualTo("built"));
            Assert.That(command.PresentTense, Is.EqualTo("building"));
            Assert.That(command.InfinitiveTense, Is.EqualTo("build"));

            bool built = false;
            command.InvokeCommand(() => built = true, () => Assert.Fail("didn't expect this to be invoked"));
            Assert.That(built);
        }

        private void AssertCleanCommandProperties(IBounceCommand command) {
            Assert.That(command.CommandLineCommand, Is.EqualTo("clean"));
            Assert.That(command.PastTense, Is.EqualTo("cleaned"));
            Assert.That(command.PresentTense, Is.EqualTo("cleaning"));
            Assert.That(command.InfinitiveTense, Is.EqualTo("clean"));

            bool cleaned = false;
            command.InvokeCommand(() => Assert.Fail("didn't expect this to be invoked"), () => cleaned = true);
            Assert.That(cleaned);
        }

        [Test]
        public void CleanCommandTest() {
            IBounceCommand cleanCommand = new BounceCommandParser().Parse("clean");
            AssertCleanCommandProperties(cleanCommand);

            var cleanAfterBuildCommand = cleanCommand.CleanAfterBuildCommand;
            Assert.That(cleanAfterBuildCommand, Is.Null);
        }
    }
}