using System.IO;
using Bounce.Framework.Obsolete;
using Bounce.Framework.Tests.Obsolete.Attributes;
using NUnit.Framework;

namespace Bounce.Framework.Tests.Obsolete.Integration {
    [TestFixture]
    [Slow]
    class FrameworkTest {
        private static string FrameworkTestFolder = "framework.tests/";
        private static string GitCheckoutDirectory = FrameworkTestFolder + "checkout";

        [SetUp]
        public void Setup() {
            if (Directory.Exists(GitCheckoutDirectory)) {
                Directory.Delete(GitCheckoutDirectory, true);
            }
        }

        [Test]
        public void AbleToUseBounceFromOutside() {

            // arrange
            var bounce = new BounceFactory().GetBounce();
            var targetsRetriever = new TargetsRetriever();
            var parameters = new CommandLineParameters();

            var targets = targetsRetriever.GetTargetsFromObject(GetTargets(parameters)).ToTargets();
            var command = BounceCommandFactory.GetCommandByName("build");
            var builder = new TargetsBuilder();

            // act
            builder.BuildTargets(bounce, targets, command);

            // assert
            Assert.That(Directory.Exists(GitCheckoutDirectory), Is.True);
        }

        public static object GetTargets(IParameters parameters) {
            var dir = new CleanDirectory {
                Path = GitCheckoutDirectory
            };

            return new {
                Directory = dir
            };
        }
    }
}
