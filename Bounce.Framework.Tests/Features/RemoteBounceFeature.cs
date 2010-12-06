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
            new BounceRunner().Run(new[] {"build", "One", "/hack:refactor", "/two:three", "/machine:live"}, method);

            Assert.That(Output.ToString(), Is.EqualTo("rexec -h live bounce.exe /describe-tasks:false /loglevel:warning /command-output:false build RemoteOne /hack:refactor\r\n"));
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

                var one = new RemoteExec
                          {
                              BounceArguments = remoteBounce.ArgumentsForTargets(new { RemoteOne = remoteOne }),
                              Machine = parameters.Required<string>("machine"),
                          };

                return remoteBounce.WithRemoteTargets(new {
                    One = one,
                    Two = two,
                });
            }
        }

        class RemoteExec : Task
        {
            [Dependency] public Future<string> BounceArguments;
            [Dependency] public Future<string> Machine;

            public override void Build() {
                Output.WriteLine("rexec -h {0} bounce.exe {1}", Machine.Value, BounceArguments.Value);
            }
        }
    }
}