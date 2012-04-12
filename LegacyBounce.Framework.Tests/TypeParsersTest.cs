using System;
using System.Globalization;
using NUnit.Framework;

namespace LegacyBounce.Framework.Tests {
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

        [Test]
        public void ShouldParseString() {
            AssertParsesAndGenerates("one", "one");
        }

        [Test]
        public void ShouldParseInteger() {
            AssertParsesAndGenerates("15", 15);
        }

        [Test]
        public void ShouldParseBoolean() {
            AssertParsesAndGenerates("true", true);
        }

        [Test]
        public void ShouldParseDate() {
            AssertParsesAndGenerates("2010-05-03 14:32:02", new DateTime(2010, 5, 3, 14, 32, 2));
        }

        private void AssertParsesAndGenerates<T>(string stringRepresentation, T internalRepresentation) {
            Assert.That(TypeParsers.Default.Parse<T>(stringRepresentation), Is.EqualTo(internalRepresentation));
            Assert.That(TypeParsers.Default.Generate(internalRepresentation), Is.EqualTo(stringRepresentation));
        }

        public class HexParser : ITypeParser {
            public object Parse(string s) {
                return int.Parse(s, NumberStyles.HexNumber);
            }

            public string Generate(object o) {
                return ((int) o).ToString("x");
            }

            public string Description {
                get { return "hex"; }
            }
        }
    }
}