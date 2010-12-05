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
            new BounceRunner().Run(new[] {"build", "RemoteOne", "/hack:refactor", "/two:three"}, method);

            Assert.That(Output.ToString(), Is.EqualTo("refactor\r\n"));
        }

        private static TextWriter Output;

        class TargetsProvider {
            [Targets]
            public static object GetTargets(IParameters parameters) {
                var remoteOne = new PrintTask(Output) {Description = parameters.Required<string>("hack")};
                var two = new PrintTask(Output) {Description = parameters.Required<string>("two")};

                var remoteBounce = new RemoteBounce {
                    PathToBounceOnRemoteMachine = "bounce.exe",
                    RemoteProcessExecutor = new RemoteProcessPrinter()
                };

                var one = remoteBounce.Targets(new {RemoteOne = remoteOne});

                return remoteBounce.WithRemoteTargets(new {
                    One = one,
                    Two = two,
                });
            }
        }

        class RemoteProcessPrinter : IRemoteProcessExecutor {
            public void ExecuteRemoteProcess(string command, string arguments) {
                Output.WriteLine("{0} {1}", command, arguments);
            }
        }
    }
}