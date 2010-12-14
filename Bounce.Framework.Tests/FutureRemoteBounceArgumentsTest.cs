using System;
using System.Linq;
using Moq;
using NUnit.Framework;

namespace Bounce.Framework.Tests {
    [TestFixture]
    public class FutureRemoteBounceArgumentsTest {
        [Test]
        public void ShouldInvokeBounceBuildWithTargets() {
            AssertBounceCommand((remoteBounceTask, bounce) => remoteBounceTask.Invoke(BounceCommand.Build, bounce), "build");
        }

        [Test]
        public void ShouldInvokeBounceCleanWithTargets() {
            AssertBounceCommand((remoteBounceTask, bounce) => remoteBounceTask.Invoke(BounceCommand.Clean, bounce), "clean");
        }

        [Test]
        public void CanBuildWithOverridingParameters() {
            var args = new FutureRemoteBounceArguments() {Targets = new {One = new Mock<ITask>().Object}};
            IParameter param1 = new Parameter<string> {Name = "name1"};
            IParameter param2 = new Parameter<string> {Name = "name2"};

            var argsWithParam1 = args.WithRemoteParameter(param1, "value1");
            var argsWithParam2 = args.WithRemoteParameter(param2, "value2");
            Assert.That(argsWithParam1.Parameters.Count(), Is.EqualTo(1));
            Assert.That(argsWithParam1.Parameters.ElementAt(0).Name, Is.EqualTo("name1"));
            Assert.That(((Parameter<string>) argsWithParam1.Parameters.ElementAt(0)).Value, Is.EqualTo("value1"));

            Assert.That(argsWithParam2.Parameters.Count(), Is.EqualTo(1));
            Assert.That(argsWithParam2.Parameters.ElementAt(0).Name, Is.EqualTo("name2"));
            Assert.That(((Parameter<string>) argsWithParam2.Parameters.ElementAt(0)).Value, Is.EqualTo("value2"));

            Assert.That(args.Parameters, Is.Empty);
        }

        private void AssertBounceCommand(Action<FutureRemoteBounceArguments, IBounce> commandAction, string command) {
            var bounce = new FakeBounce();

            var a = new Mock<ITask>().Object;
            var b = new Mock<ITask>().Object;
            var overridingParameters = new[] {new Mock<IParameter>().Object};

            var logOptionTranslator = new Mock<ILogOptionCommandLineTranslator>();
            logOptionTranslator.Setup(l => l.GenerateCommandLine(bounce)).Returns("logoptions");

            var commandLineParametersGenerator = new Mock<ICommandLineTasksParametersGenerator>();
            commandLineParametersGenerator.Setup(c => c.GenerateCommandLineParametersForTasks(new [] {a, b}, overridingParameters)).Returns("build_arguments_and_params");

            var remoteBounce = new FutureRemoteBounceArguments(new TargetsParser(), logOptionTranslator.Object, commandLineParametersGenerator.Object);
            remoteBounce.Targets = new {Junk = a, Aspr = b};
            remoteBounce.Parameters = overridingParameters;

            commandAction(remoteBounce, bounce);
            Assert.That(remoteBounce.Value, Is.EqualTo("logoptions " + command + " Junk Aspr build_arguments_and_params"));
        }
    }
}