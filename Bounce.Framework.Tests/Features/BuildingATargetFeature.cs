using System;
using System.IO;
using System.Reflection;
using NUnit.Framework;

namespace Bounce.Framework.Tests.Features {
    [TestFixture]
    public class BuildingATargetFeature {
        [SetUp]
        public void SetUp() {
            Output = new StringWriter();
        }

        [Test]
        public void ShouldGenerateCommandLineForRemoteBounce() {
            MethodInfo method = typeof (TargetsProvider).GetMethod("GetTargets");
            new BounceRunner().Run(new[] {"build", "One"}, method);

            Assert.That(Output.ToString(), Is.EqualTo("one\r\n"));
        }

        private static TextWriter Output;

        class TargetsProvider {
            [Targets]
            public static object GetTargets() {
                var one = new PrintTask(Output) { Description = "one" };

                return new {
                    One = one,
                };
            }
        }
    }
}