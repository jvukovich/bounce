using Bounce.Framework.Iis;
using NUnit.Framework;

namespace Bounce.Framework.Tests.Iis
{
    [TestFixture]
    public class BindingParserTest {
        [Test]
        public void ParsesHttpNoPortWithHost() {
            var parser = new BindingParser();
            var binding = parser.Parse("http://example.com/");

            Assert.That(binding.Port, Is.EqualTo(80));
            Assert.That(binding.Host, Is.EqualTo("example.com"));
            Assert.That(binding.Protocol, Is.EqualTo("http"));
            Assert.That(binding.Path, Is.Null);
        }

        [Test]
        public void ParsesHttpNoPortWithoutHost() {
            var parser = new BindingParser();
            var binding = parser.Parse("http://*/");

            Assert.That(binding.Port, Is.EqualTo(80));
            Assert.That(binding.Host, Is.Null);
            Assert.That(binding.Protocol, Is.EqualTo("http"));
            Assert.That(binding.Path, Is.Null);
        }

        [Test]
        public void ParsesHttpPort4000WithoutHost() {
            var parser = new BindingParser();
            var binding = parser.Parse("http://*:4000/");

            Assert.That(binding.Port, Is.EqualTo(4000));
            Assert.That(binding.Host, Is.Null);
            Assert.That(binding.Protocol, Is.EqualTo("http"));
            Assert.That(binding.Path, Is.Null);
        }

        [Test]
        public void ParsesHttpPort4000WithHost() {
            var parser = new BindingParser();
            var binding = parser.Parse("http://example.com:4000/");

            Assert.That(binding.Port, Is.EqualTo(4000));
            Assert.That(binding.Host, Is.EqualTo("example.com"));
            Assert.That(binding.Protocol, Is.EqualTo("http"));
            Assert.That(binding.Path, Is.Null);
        }

        [Test]
        public void ParsesHttpsNoPortWithHost() {
            var parser = new BindingParser();
            var binding = parser.Parse("https://example.com/");

            Assert.That(binding.Port, Is.EqualTo(443));
            Assert.That(binding.Host, Is.EqualTo("example.com"));
            Assert.That(binding.Protocol, Is.EqualTo("https"));
            Assert.That(binding.Path, Is.Null);
        }

        [Test]
        public void ParsesHttpsPort4000WithHostAndPath() {
            var parser = new BindingParser();
            var binding = parser.Parse("https://example.com:4000/path/to/website");

            Assert.That(binding.Port, Is.EqualTo(4000));
            Assert.That(binding.Host, Is.EqualTo("example.com"));
            Assert.That(binding.Protocol, Is.EqualTo("https"));
            Assert.That(binding.Path, Is.EqualTo("/path/to/website"));
        }
    }
}
