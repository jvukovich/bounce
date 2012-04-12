using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Bounce.Framework.Tests
{
    [TestFixture]
    public class ParameterParserTest {
        [Test]
        public void ShouldParseNoParameters() {
            var parser = new ArgumentsParser();
            var paramDict = parser.ParseParameters(new string[0]);

            Assert.That(paramDict, Is.Empty);
        }

        [Test]
        public void ShouldParseParametersWithColons() {
            var parser = new ArgumentsParser();
            var paramDict = parser.ParseParameters(Params("/file:afile.txt /name:nobody"));

            Assert.That(paramDict["file"], Is.EqualTo("afile.txt"));
            Assert.That(paramDict["name"], Is.EqualTo("nobody"));
        }

        [Test]
        public void ShouldParseParametersWithSpaces() {
            var parser = new ArgumentsParser();
            var paramDict = parser.ParseParameters(Params("/file afile.txt /name nobody"));

            Assert.That(paramDict["file"], Is.EqualTo("afile.txt"));
            Assert.That(paramDict["name"], Is.EqualTo("nobody"));
        }

        [Test]
        public void ShouldParseBooleanParameterAtEnd() {
            var parser = new ArgumentsParser();
            var paramDict = parser.ParseParameters(Params("/file:afile.txt /fast"));

            Assert.That(paramDict["file"], Is.EqualTo("afile.txt"));
            Assert.That(paramDict["fast"], Is.EqualTo("true"));
        }

        [Test]
        public void ShouldParseBooleanParameterInMiddle() {
            var parser = new ArgumentsParser();
            var paramDict = parser.ParseParameters(Params("/fast /file:afile.txt"));

            Assert.That(paramDict["file"], Is.EqualTo("afile.txt"));
            Assert.That(paramDict["fast"], Is.EqualTo("true"));
        }

        [Test]
        public void ThrowsIfNonNamedArgumentFound() {
            var parser = new ArgumentsParser();
            Assert.That(() => parser.ParseParameters(Params("afile.txt")), Throws.InstanceOf<NonNamedArgumentException>());
        }

        private IEnumerable<string> Params(string parameters) {
            return parameters.Split(' ');
        }
    }
}
