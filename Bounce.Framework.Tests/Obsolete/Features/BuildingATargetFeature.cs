using System.IO;
using System.Reflection;
using Bounce.Framework.Obsolete;
using NUnit.Framework;

namespace Bounce.Framework.Tests.Obsolete.Features {
    [TestFixture]
    public class BuildingATargetFeature {
        [SetUp]
        public void SetUp() {
            Output = new StringWriter();
        }

        [Test]
        public void ShouldBuildOneTarget() {
            MethodInfo method = typeof (TargetsProvider).GetMethod("GetTargets");
            new ObsoleteBounceRunner().Run(new[] {"build", "One"}, method);

            Assert.That(Output.ToString(), Is.EqualTo("one\r\n"));
        }

        [Test]
        public void ShouldBuildTargetByDefaultIfNoCommandGiven() {
            MethodInfo method = typeof (TargetsProvider).GetMethod("GetTargets");
            new ObsoleteBounceRunner().Run(new[] { "One" }, method);

            Assert.That(Output.ToString(), Is.EqualTo("one\r\n"));
        }

        [Test]
        public void ShouldBuildMoreThanOneTarget() {
            MethodInfo method = typeof (TargetsProvider).GetMethod("GetTargets");
            new ObsoleteBounceRunner().Run(new[] { "build", "One", "Two" }, method);

            Assert.That(Output.ToString(), Is.EqualTo("one\r\ntwo\r\n"));
        }

        [Test]
        public void ShouldBuildOneTargetEvenIfAnotherTargetRequiresParametersThatAreNotSet() {
            MethodInfo method = typeof (TargetsProviderWithRequiredParameters).GetMethod("GetTargets");
            new ObsoleteBounceRunner().Run(new[] { "build", "One", "/one:thisisone" }, method);

            Assert.That(Output.ToString(), Is.EqualTo("thisisone\r\n"));
        }

        private static TextWriter Output;

        class TargetsProvider {
            [Targets]
            public static object GetTargets() {
                var one = new PrintTask(Output) { Description = "one" };
                var two = new PrintTask(Output) { Description = "two" };

                return new {
                    One = one,
                    Two = two,
                };
            }
        }

        class TargetsProviderWithRequiredParameters {
            [Targets]
            public static object GetTargets(IParameters parameters) {
                var one = new PrintTask(Output) { Description = parameters.Required<string>("one") };
                var two = new PrintTask(Output) { Description = parameters.Required<string>("two") };

                return new {
                    One = one,
                    Two = two,
                };
            }
        }
    }
}