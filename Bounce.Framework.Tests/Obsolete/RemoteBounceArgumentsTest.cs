using System;
using System.Linq;
using Bounce.Framework.Obsolete;
using Moq;
using NUnit.Framework;

namespace Bounce.Framework.Tests.Obsolete {
    [TestFixture]
    public class RemoteBounceArgumentsTest {
        [Test]
        public void ShouldInvokeBounceBuildWithTargets() {
            AssertBounceCommand((remoteBounceTask, bounce) => remoteBounceTask.Invoke(new BounceCommandParser().Build, bounce), "build");
        }

        [Test]
        public void ShouldInvokeBounceBuildAndKeepWithTargets() {
            AssertBounceCommand((remoteBounceTask, bounce) => remoteBounceTask.Invoke(new BounceCommandParser().BuildAndClean, bounce), "buildandclean");
        }

        [Test]
        public void ShouldInvokeBounceCleanWithTargets() {
            AssertBounceCommand((remoteBounceTask, bounce) => remoteBounceTask.Invoke(new BounceCommandParser().Clean, bounce), "clean");
        }

        [Test]
        public void CanBuildWithOverridingParameters() {
            var args = new RemoteBounceArguments() {Targets = new [] {"One"}};
            var param1 = new Parameter<string> {Name = "name1"};
            var param2 = new Parameter<string> { Name = "name2" };

            var argsWithParam1 = args.WithParameter(param1.WithValue("value1"));
            var argsWithParam2 = args.WithParameter(param2.WithValue("value2"));
            Assert.That(argsWithParam1.Parameters.Count(), Is.EqualTo(1));
            Assert.That(argsWithParam1.Parameters.ElementAt(0).Name, Is.EqualTo("name1"));
            Assert.That(((Parameter<string>) argsWithParam1.Parameters.ElementAt(0)).Value, Is.EqualTo("value1"));

            Assert.That(argsWithParam2.Parameters.Count(), Is.EqualTo(1));
            Assert.That(argsWithParam2.Parameters.ElementAt(0).Name, Is.EqualTo("name2"));
            Assert.That(((Parameter<string>) argsWithParam2.Parameters.ElementAt(0)).Value, Is.EqualTo("value2"));

            Assert.That(args.Parameters, Is.Empty);
        }

        private void AssertBounceCommand(Action<RemoteBounceArguments, IBounce> commandAction, string command) {
            var bounce = new FakeBounce();
            bounce.ParametersGiven = new[] {new Parameter<string>("name", "value")};

            var a = new Mock<IObsoleteTask>().Object;
            var b = new Mock<IObsoleteTask>().Object;
            var overridingParameters = new[] {new Mock<IParameter>().Object};

            var logOptionTranslator = new Mock<ILogOptionCommandLineTranslator>();
            logOptionTranslator.Setup(l => l.GenerateCommandLine(bounce)).Returns("logoptions");

            var commandLineParametersGenerator = new Mock<ICommandLineTasksParametersGenerator>();
            commandLineParametersGenerator.Setup(c => c.GenerateCommandLineParametersForTasks(bounce.ParametersGiven, overridingParameters)).Returns("build_arguments_and_params");

            var remoteBounce = new RemoteBounceArguments(new TargetsParser(), logOptionTranslator.Object, commandLineParametersGenerator.Object);
            remoteBounce.Targets = new [] {"Junk"};
            remoteBounce.Parameters = overridingParameters;

            commandAction(remoteBounce, bounce);
            Assert.That(remoteBounce.Value, Is.EqualTo("logoptions " + command + " Junk build_arguments_and_params"));
        }
    }
}