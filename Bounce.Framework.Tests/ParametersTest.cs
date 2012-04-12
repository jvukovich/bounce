using System.Collections.Generic;
using NUnit.Framework;

namespace Bounce.Framework.Tests {
    [TestFixture]
    public class ParametersTest {
        [Test]
        public void CanParseDefaultStringParameter() {
            var p = new Parameters(new Dictionary<string, string> {{"file", "thefile.txt"}});
            Assert.That(p.Parameter("file", "afile.txt"), Is.EqualTo("thefile.txt"));
        }

        [Test]
        public void CanParseDefaultStringParameterIfNotPresent() {
            var p = new Parameters(new Dictionary<string, string>());
            Assert.That(p.Parameter("file", "afile.txt"), Is.EqualTo("afile.txt"));
        }

        [Test]
        public void CanParseStringParameter() {
            var p = new Parameters(new Dictionary<string, string> { { "file", "thefile.txt" } });
            Assert.That(p.Parameter("file", "afile.txt"), Is.EqualTo("thefile.txt"));
        }

        [Test]
        public void CanParseDefaultStringParameterWithType() {
            var p = new Parameters(new Dictionary<string, string> {{"file", "thefile.txt"}});
            Assert.That(p.Parameter(typeof(string), "file", "afile.txt"), Is.EqualTo("thefile.txt"));
        }

        [Test]
        public void CanParseDefaultStringParameterIfNotPresentWithType() {
            var p = new Parameters(new Dictionary<string, string>());
            Assert.That(p.Parameter(typeof(string), "file", "afile.txt"), Is.EqualTo("afile.txt"));
        }

        [Test]
        public void CanParseStringParameterWithType() {
            var p = new Parameters(new Dictionary<string, string> { { "file", "thefile.txt" } });
            Assert.That(p.Parameter(typeof(string), "file", "afile.txt"), Is.EqualTo("thefile.txt"));
        }

        [Test]
        public void ThrowsExceptionWhenStringParameterIsNotPresentWithType()
        {
            var p = new Parameters(new Dictionary<string, string>());
            Assert.That(() => p.Parameter(typeof(string), "file"), Throws.InstanceOf<RequiredParameterNotGivenException>());
        }

        [Test]
        public void ThrowsExceptionWhenStringParameterIsNotPresent()
        {
            var p = new Parameters(new Dictionary<string, string>());
            Assert.That(() => p.Parameter<string>("file"), Throws.InstanceOf<RequiredParameterNotGivenException>());
        }

        [Test]
        public void CanParseEnumeration() {
            var p = new Parameters(new Dictionary<string, string> { { "lake", "constance" } });
            Assert.That(p.Parameter<Lakes>("lake"), Is.EqualTo(Lakes.Constance));
        }

        enum Lakes {
            Constance,
            Coniston,
            Consequence
        }
    }
}