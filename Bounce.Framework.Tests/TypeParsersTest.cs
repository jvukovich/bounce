using System;
using System.Globalization;
using NUnit.Framework;

namespace Bounce.Framework.Tests {
    [TestFixture]
    public class TypeParsersTest {
        [Test]
        public void ShouldParseRegisteredType() {
            var parsers = new TypeParsers();
            parsers.RegisterTypeParser<int>(new HexParser());
            Assert.That(parsers.Parse<int>("10"), Is.EqualTo(16));
        }

        [Test]
        public void ShouldGenerateRegisteredType() {
            var parsers = new TypeParsers();
            parsers.RegisterTypeParser<int>(new HexParser());
            Assert.That(parsers.Generate(16), Is.EqualTo("10"));
        }

        public class HexParser : ITypeParser {
            public object Parse(string s) {
                return int.Parse(s, NumberStyles.HexNumber);
            }

            public string Generate(object o) {
                return ((int) o).ToString("x");
            }
        }
    }
}