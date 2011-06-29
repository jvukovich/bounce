using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.IO;
using Bounce.Framework.Tests.Attributes;

namespace Bounce.Framework.Tests.Integration {
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

        [Test, Ignore]
        public void AbleToUseBounceFromOutside() {

            // arrange
            var bounce = BounceFactory.GetBounce();
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
