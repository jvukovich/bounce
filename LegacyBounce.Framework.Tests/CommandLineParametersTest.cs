using System.Collections.Generic;
using NUnit.Framework;

namespace LegacyBounce.Framework.Tests {
    [TestFixture]
    public class CommandLineParametersTest {
        [Test]
        public void RequiredShouldReturnValueWhenGiven() {
            var parms = new CommandLineParameters();
            var version = parms.Required<string>("version");

            parms.ParseCommandLineArguments(new List<ParsedCommandLineParameter> {new ParsedCommandLineParameter {Name = "version", Value = "0.2"}});

            Assert.That(version.Value, Is.EqualTo("0.2"));
        }

        [Test]
        public void RequiredShouldThrowExceptionIfNoValueGiven() {
            var parms = new CommandLineParameters();

            var param1 = (IParameter) parms.Required<string>("provided");
            var param2 = (IParameter) parms.Required<int>("notprovided");

            var parsedCommandLineParameters = new List<ParsedCommandLineParameter>();
            parsedCommandLineParameters.Add(new ParsedCommandLineParameter {Name = "provided", Value = "value"});
            parms.ParseCommandLineArguments(parsedCommandLineParameters);

            Assert.That(() => parms.EnsureAllRequiredParametersHaveValues(new [] {param1, param2}), Throws.InstanceOf(typeof(CommandLineParametersException)));
        }

        [Test]
        public void ShouldThrowIfParameterGivenButNoneRegistered() {
            var parms = new CommandLineParameters();

            var parsedCommandLineParameters = new List<ParsedCommandLineParameter>();
            parsedCommandLineParameters.Add(new ParsedCommandLineParameter {Name = "provided", Value = "value"});
            Assert.That(() => parms.ParseCommandLineArguments(parsedCommandLineParameters), Throws.InstanceOf(typeof(CommandLineParametersException)));
        }

        [Test]
        public void RequiredValueShouldThrowIfNotParsedYet() {
            var parms = new CommandLineParameters();
            var version = parms.Required<string>("version");

            Assert.That(() => version.Value, Throws.Exception);
        }

        [Test]
        public void DefaultValueShouldReturnIfNotParsed() {
            var parms = new CommandLineParameters();
            var version = parms.Default("version", "0.2");

            Assert.That(version.Value, Is.EqualTo("0.2"));
        }
    }
}