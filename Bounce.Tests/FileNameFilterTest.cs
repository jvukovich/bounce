using System.Text.RegularExpressions;
using Bounce.Framework;
using Moq;
using NUnit.Framework;

namespace Bounce.Tests {
    [TestFixture]
    public class FileNameFilterTest {
        [Test]
        public void ShouldExcludeFilesMatchingExcludeListButNotInIncludesList() {
            var globToRegex = new Mock<IGlobToRegexConverter>();
            globToRegex.Setup(g => g.ConvertToRegex("*.xml")).Returns(new Regex(@".*\.xml$"));
            globToRegex.Setup(g => g.ConvertToRegex("include.xml")).Returns(new Regex(@"include\.xml$"));

            var excluder = new FileNameFilter(globToRegex.Object, new [] {"*.xml"}, new [] {"include.xml"});
            Assert.That(excluder.IncludeFile("test.xml"), Is.EqualTo(false));
            Assert.That(excluder.IncludeFile("include.xml"), Is.EqualTo(true));
            Assert.That(excluder.IncludeFile("test.txt"), Is.EqualTo(true));
        }
    }
}