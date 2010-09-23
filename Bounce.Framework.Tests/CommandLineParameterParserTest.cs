using NUnit.Framework;

namespace Bounce.Framework.Tests {
    [TestFixture]
    public class CommandLineParameterParserTest {
        [Test]
        public void ShouldParseCommandLineParameters() {
            var parser = new CommandLineParameterParser();
            var parms = parser.ParseCommandLineParameters(new[] {"zero", "/one", @"c:\two", "/three", "4", "http://five"});

            AssertArgumentsParsed(parms);
        }

        [Test]
        public void ShouldParseCommandLineParametersWithColons() {
            var parser = new CommandLineParameterParser();
            var parms = parser.ParseCommandLineParameters(new[] {"zero", @"/one:c:\two", "/three:4", "http://five"});

            AssertArgumentsParsed(parms);
        }

        private void AssertArgumentsParsed(ParsedCommandLineParameters parms) {
            Assert.That(parms.Parameters[0].Name, Is.EqualTo("one"));
            Assert.That(parms.Parameters[0].Value, Is.EqualTo(@"c:\two"));
            Assert.That(parms.Parameters[1].Name, Is.EqualTo("three"));
            Assert.That(parms.Parameters[1].Value, Is.EqualTo("4"));
            Assert.That(parms.RemainingArguments, Is.EqualTo(new[] {"zero", "http://five"}));
        }
    }
}