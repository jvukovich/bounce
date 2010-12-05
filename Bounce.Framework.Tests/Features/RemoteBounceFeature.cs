using System.IO;
using System.Reflection;
using NUnit.Framework;

namespace Bounce.Framework.Tests.Features {
    [TestFixture]
    public class RemoteBounceFeature {
        [SetUp]
        public void SetUp() {
            Output = new StringWriter();
        }

        [Test]
        public void ShouldGenerateCommandLineForRemoteBounceWithParametersUsedInRemoteTarget() {
            MethodInfo method = typeof (TargetsProvider).GetMethod("GetTargets");
            new BounceRunner().Run(new[] {"build", "One", "/hack:refactor", "/two:three"}, method);

            Assert.That(Output.ToString(), Is.EqualTo("bounce.exe /describe-tasks:false /loglevel:warning /command-output:false RemoteOne /hack:refactor\r\n"));
        }

        [Test]
        public void ShouldIncludeRemoteTargetsSoTheyCanBeInvokedRemotely() {
            MethodInfo method = typeof (TargetsProvider).GetMethod("GetTargets");
            new BounceRunner().Run(new[] {"build", "RemoteOne", "/hack:refactor"}, method);

            Assert.That(Output.ToString(), Is.EqualTo("refactor\r\n"));
        }

        private static TextWriter Output;

        class TargetsProvider {
            [Targets]
            public static object GetTargets(IParameters parameters) {
                var remoteOne = new PrintTask(Output) {Description = parameters.Required<string>("hack")};
                var two = new PrintTask(Output) {Description = parameters.Required<string>("two")};

                var remoteBounce = new RemoteBounce();

                var one = remoteBounce.Targets(new {RemoteOne = remoteOne}, new RemoteBouncePrinter());

                return remoteBounce.WithRemoteTargets(new {
                    One = one,
                    Two = two,
                });
            }
        }

        class RemoteBouncePrinter : IRemoteBounceExecutor {
            public void ExecuteRemoteBounce(string arguments) {
                Output.WriteLine("bounce.exe " + arguments);
            }
        }
    }
}