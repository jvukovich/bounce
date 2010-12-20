using NUnit.Framework;

namespace Bounce.Framework.Tests {
    [TestFixture]
    public class BounceCommandExtensionsTest {
        [Test]
        public void ShouldInvokeCleanActionOnCleanCommand() {
            bool cleaned = false;
            BounceCommand.Clean.InvokeCommand(null, () => cleaned = true);

            Assert.That(cleaned);
        }

        [Test]
        public void ShouldInvokeBuildActionOnBuildCommand() {
            bool built = false;
            BounceCommand.Build.InvokeCommand(() => built = true, null);

            Assert.That(built);
        }

        [Test]
        public void ShouldInvokeBuildActionOnBuildAndKeepCommand() {
            bool built = false;
            BounceCommand.BuildAndKeep.InvokeCommand(() => built = true, null);

            Assert.That(built);
        }

        [Test]
        public void CleanAfterBuildReturnsTrueIffBuildButNotCleanOrBuildAndKeep() {
            Assert.That(BounceCommand.Build.CleanAfterBuild(), Is.True);
            Assert.That(BounceCommand.Clean.CleanAfterBuild(), Is.False);
            Assert.That(BounceCommand.BuildAndKeep.CleanAfterBuild(), Is.False);
        }

    }
}