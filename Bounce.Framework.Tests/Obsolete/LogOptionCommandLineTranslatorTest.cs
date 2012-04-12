using System;
using Bounce.Framework.Obsolete;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace Bounce.Framework.Tests.Obsolete {
    [TestFixture]
    public class LogOptionCommandLineTranslatorTest {
        private LogFactoryRegistry LogFactoryRegistry;

        [SetUp]
        public void SetUp() {
            LogFactoryRegistry = new LogFactoryRegistry();
        }

        [Test]
        public void ShouldSetLogLevelFromParameter() {
            AssertThatParameterSetsOption(LogOptionCommandLineTranslator.LogLevelParameter, "info", b => b.LogOptions.LogLevel, LogLevel.Info);
        }

        [Test]
        public void ShouldSetCommandOutputFromParameter() {
            AssertThatParameterSetsOption(LogOptionCommandLineTranslator.CommandOutputParameter, "true", b => b.LogOptions.CommandOutput, true);
        }

        [Test]
        public void ShouldSetLogFormatFromParameter() {
            ITaskLogFactory myLogger = new Mock<ITaskLogFactory>().Object;
            LogFactoryRegistry.RegisterLogFactory("mylogger", myLogger);

            AssertThatParameterSetsOption(LogOptionCommandLineTranslator.LogFormatParameter, "mylogger", b => b.LogFactory, myLogger);
        }

        [Test]
        public void ShouldSetDescribeTasksFromParameter() {
            AssertThatParameterSetsOption(LogOptionCommandLineTranslator.DescribeTasksParameter, "true", b => b.LogOptions.DescribeTasks, true);
        }

        [Test]
        public void ShouldGenerateCommandLineParametersFromOptions() {
            ITaskLogFactory myLogger = new Mock<ITaskLogFactory>().Object;
            LogFactoryRegistry.RegisterLogFactory("mylogger", myLogger);

            var translator = new LogOptionCommandLineTranslator(LogFactoryRegistry);

            var bounce = new FakeBounce();

            bounce.LogFactory = myLogger;
            bounce.LogOptions.DescribeTasks = true;
            bounce.LogOptions.CommandOutput = false;
            bounce.LogOptions.LogLevel = LogLevel.Warning;

            var commandLine = translator.GenerateCommandLine(bounce);

            Assert.That(commandLine, ContainsArgument("/describe-tasks:true"));
            Assert.That(commandLine, ContainsArgument("/loglevel:warning"));
            Assert.That(commandLine, ContainsArgument("/command-output:false"));
            Assert.That(commandLine, ContainsArgument("/logformat:mylogger"));
        }

        [Test]
        public void ShouldGenerateCommandLineParametersFromOptionsWithoutLogFormatWhenNotFound() {
            ITaskLogFactory myLogger = new Mock<ITaskLogFactory>().Object;

            var translator = new LogOptionCommandLineTranslator(LogFactoryRegistry);

            var bounce = new FakeBounce();

            bounce.LogFactory = myLogger;
            bounce.LogOptions.DescribeTasks = true;
            bounce.LogOptions.CommandOutput = false;
            bounce.LogOptions.LogLevel = LogLevel.Warning;

            var commandLine = translator.GenerateCommandLine(bounce);

            Assert.That(commandLine, ContainsArgument("/describe-tasks:true"));
            Assert.That(commandLine, ContainsArgument("/loglevel:warning"));
            Assert.That(commandLine, ContainsArgument("/command-output:false"));
            Assert.That(commandLine, Is.Not.StringMatching(@"(^|\s)/logformat:"));
        }

        private RegexConstraint ContainsArgument(string arg) {
            return Is.StringMatching(@"(^|\s)" + arg + @"($|\s)");
        }

        private void AssertThatParameterSetsOption(string parameterName, string parameterValue, Func<IBounce, object> option, object expectedValue) {
            var parameters = new ParsedCommandLineParameters();
            parameters.Parameters.Add(new ParsedCommandLineParameter {Name = parameterName, Value = parameterValue});

            var bounce = new FakeBounce();

            new LogOptionCommandLineTranslator(LogFactoryRegistry).ParseCommandLine(parameters, bounce);

            Func<IBounce, object> commandOutput = option;
            Assert.That(commandOutput(bounce), Is.EqualTo(expectedValue));
        }
    }
}