using System.Collections.Generic;
using NUnit.Framework;

namespace Bounce.Framework.Tests {
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
            var version = parms.Required<string>("version");

            Assert.That(() => parms.ParseCommandLineArguments(new List<ParsedCommandLineParameter>()), Throws.InstanceOf(typeof(CommandLineParametersException)));
        }
    }
}