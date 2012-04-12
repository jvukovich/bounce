using Bounce.Framework.Obsolete;
using NUnit.Framework;

namespace Bounce.Framework.Tests.Obsolete {
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
        public void ShouldInvokeBuildActionOnBuildAndCleanCommand() {
            bool built = false;
            BounceCommand.BuildAndClean.InvokeCommand(() => built = true, null);

            Assert.That(built);
        }
    }
}