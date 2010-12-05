using Moq;
using NUnit.Framework;

namespace Bounce.Framework.Tests {
    [TestFixture]
    public class RemoteBounceTaskTest {
        [Test]
        public void ShouldInvokeBounceWithTargets() {
            var remoteExecutor = new Mock<IRemoteBounceExecutor>();
            var bounce = new FakeBounce();

            var a = new Mock<ITask>().Object;
            var b = new Mock<ITask>().Object;

            var logOptionTranslator = new Mock<ILogOptionCommandLineTranslator>();
            logOptionTranslator.Setup(l => l.GenerateCommandLine(bounce)).Returns("logoptions");

            var commandLineParametersGenerator = new Mock<ICommandLineTasksParametersGenerator>();
            commandLineParametersGenerator.Setup(c => c.GenerateCommandLineParametersForTasks(new [] {a, b})).Returns("buildarguments");

            var remoteBounce = new RemoteBounceTask(new TargetsParser(), logOptionTranslator.Object, commandLineParametersGenerator.Object);

            remoteBounce.RemoteBounceExecutor = remoteExecutor.Object;

            remoteBounce.Targets = new {Junk = a, Aspr = b};

            remoteBounce.Build(bounce);

            remoteExecutor.Verify(r => r.ExecuteRemoteBounce("logoptions Junk Aspr buildarguments"));
        }
    }
}