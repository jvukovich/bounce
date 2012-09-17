using System.Net;
using Bounce.Framework.Web;
using NUnit.Framework;

namespace Bounce.Framework.Tests.Web
{
    [TestFixture]
    public class WebSiteBindingParserTest {
        [Test]
        public void ParsesHttpNoPortWithHost() {
            var parser = new WebSiteBindingParser();
            var binding = parser.Parse("http://example.com/");

            Assert.That(binding.Port, Is.EqualTo(80));
            Assert.That(binding.Host, Is.EqualTo("example.com"));
            Assert.That(binding.Protocol, Is.EqualTo("http"));
            Assert.That(binding.Path, Is.Null);
        }

        [Test]
        public void ParsesHttpNoPortWithoutHost() {
            var parser = new WebSiteBindingParser();
            var binding = parser.Parse("http://*/");

            Assert.That(binding.Port, Is.EqualTo(80));
            Assert.That(binding.Host, Is.Null);
            Assert.That(binding.Protocol, Is.EqualTo("http"));
            Assert.That(binding.Path, Is.Null);
        }

        [Test]
        public void ParsesHttpPort4000WithoutHost() {
            var parser = new WebSiteBindingParser();
            var binding = parser.Parse("http://*:4000/");

            Assert.That(binding.Port, Is.EqualTo(4000));
            Assert.That(binding.Host, Is.Null);
            Assert.That(binding.Protocol, Is.EqualTo("http"));
            Assert.That(binding.Path, Is.Null);
        }

        [Test]
        public void ParsesHttpPort4000WithHost() {
            var parser = new WebSiteBindingParser();
            var binding = parser.Parse("http://example.com:4000/");

            Assert.That(binding.Port, Is.EqualTo(4000));
            Assert.That(binding.Host, Is.EqualTo("example.com"));
            Assert.That(binding.Protocol, Is.EqualTo("http"));
            Assert.That(binding.Path, Is.Null);
        }

        [Test]
        public void ParsesHttpsNoPortWithHost() {
            var parser = new WebSiteBindingParser();
            var binding = parser.Parse("https://example.com/");

            Assert.That(binding.Port, Is.EqualTo(443));
            Assert.That(binding.Host, Is.EqualTo("example.com"));
            Assert.That(binding.Protocol, Is.EqualTo("https"));
            Assert.That(binding.Path, Is.Null);
        }

        [Test]
        public void ParsesHttpsPort4000WithHostAndPath() {
            var parser = new WebSiteBindingParser();
            var binding = parser.Parse("https://example.com:4000/path/to/website");

            Assert.That(binding.Port, Is.EqualTo(4000));
            Assert.That(binding.Host, Is.EqualTo("example.com"));
            Assert.That(binding.Protocol, Is.EqualTo("https"));
            Assert.That(binding.Path, Is.EqualTo("/path/to/website"));
        }

        [Test]
        public void ParsesHttpWithIpAddress() {
            var parser = new WebSiteBindingParser();
            var binding = parser.Parse("https://127.0.0.1:4000/");

            Assert.That(binding.Port, Is.EqualTo(4000));
            Assert.That(binding.IpAddress, Is.EqualTo(new IPAddress(new byte[] {127, 0, 0, 1})));
            Assert.That(binding.Host, Is.Null);
            Assert.That(binding.Protocol, Is.EqualTo("https"));
            Assert.That(binding.Path, Is.Null);
        }

        [Test]
        public void ReturnsIisInformationWithHost() {
            var binding = new WebSiteBinding {
                Host = "example.com",
                Port = 4000,
                Path = null
            };

            Assert.That(binding.Information, Is.EqualTo("*:4000:example.com"));
        }

        [Test]
        public void ReturnsIisInformationWithIp() {
            var binding = new WebSiteBinding {
                IpAddress = new IPAddress(new byte[] {127, 0, 0, 1}),
                Port = 4000,
                Path = null
            };

            Assert.That(binding.Information, Is.EqualTo("127.0.0.1:4000:"));
        }

    }
}
