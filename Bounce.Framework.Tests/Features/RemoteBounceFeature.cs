using System.Collections.Generic;
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

            string rexecCommands = "rexec -h live bounce.exe /describe-tasks:false /loglevel:warning /command-output:false build RemoteOne /hack:refactor /machine:live /two:three\r\n";

            Assert.That(Output.ToString(), Is.EqualTo(rexecCommands));
        }

        [Test]
        public void ShouldIncludeRemoteTargetsSoTheyCanBeInvokedRemotely() {
            MethodInfo method = typeof (TargetsProvider).GetMethod("GetTargets");
            new BounceRunner().Run(new[] {"build", "RemoteOne", "/hack:refactor"}, method);

            Assert.That(Output.ToString(), Is.EqualTo("refactor\r\n"));
        }

        [Test]
        public void ShouldExecuteMultipleRemoteTasksEachWithDifferentParameters() {
            MethodInfo method = typeof(MultipleRemoteTargetsProvider).GetMethod("GetTargets");
            new BounceRunner().Run(new[] {"build", "One"}, method);

            string rexecCommands =
@"rexec -h machine1 bounce.exe /describe-tasks:false /loglevel:warning /command-output:false build RemoteOne /machineName:machine1
rexec -h machine2 bounce.exe /describe-tasks:false /loglevel:warning /command-output:false build RemoteOne /machineName:machine2
";

            Assert.That(Output.ToString(), Is.EqualTo(rexecCommands));
        }

        [Test]
        public void ShouldInvokeRemoteTargetWithArguments() {
            MethodInfo method = typeof(MultipleRemoteTargetsProvider).GetMethod("GetTargets");
            new BounceRunner().Run(new[] {"build", "RemoteOne", "/machineName:machine1"}, method);

            string rexecCommands = "machine1\r\n";

            Assert.That(Output.ToString(), Is.EqualTo(rexecCommands));
        }

        private static TextWriter Output;

        class TargetsProvider {
            [Targets]
            public static object GetTargets(IParameters parameters) {
                var remoteOne = new PrintTask(Output) {Description = parameters.Required<string>("hack")};
                var two = new PrintTask(Output) {Description = parameters.Required<string>("two")};

                var remoteBounce = new RemoteBounce();

                RemoteBounceArguments remoteOneArgs = remoteBounce.ArgumentsForTargets(new { RemoteOne = remoteOne });

                var one = new RemoteExec
                          {
                              BounceArguments = remoteOneArgs,
                              Machine = parameters.Required<string>("machine"),
                          };

                return remoteBounce.WithRemoteTargets(new {
                    One = one,
                    Two = two,
                });
            }
        }

        class MultipleRemoteTargetsProvider {
            [Targets]
            public static object GetTargets(IParameters parameters) {
                var machineName = parameters.Required<string>("machineName");
                var remoteOne = new PrintTask(Output) {Description = machineName};
                var two = new PrintTask(Output) {Description = parameters.Required<string>("two")};

                var remoteBounce = new RemoteBounce();

                RemoteBounceArguments remoteOneArgs = remoteBounce.ArgumentsForTargets(new { RemoteOne = remoteOne });

                Task<IEnumerable<string>> machines = new [] {"machine1", "machine2"};
                var one = machines.SelectTasks(machine => new RemoteExec {
                    BounceArguments = remoteOneArgs.WithParameter(machineName.WithValue(machine)),
                    Machine = machine,
                });

                return remoteBounce.WithRemoteTargets(new {
                    One = one,
                    Two = two,
                });
            }
        }

        class RemoteExec : Task
        {
            [Dependency] public Task<string> BounceArguments;
            [Dependency] public Task<string> Machine;

            public override void Build() {
                Output.WriteLine("rexec -h {0} bounce.exe {1}", Machine.Value, BounceArguments.Value);
            }
        }
    }
}