using System.Text.RegularExpressions;
using Moq;
using NUnit.Framework;

namespace LegacyBounce.Framework.Tests {
    [TestFixture]
    public class FileNameFilterTest {
        [Test]
        public void ShouldExcludeFilesMatchingExcludeListButNotInIncludesList() {
            var globToRegex = new Mock<IGlobToRegexConverter>();
            globToRegex.Setup(g => g.ConvertToRegex("*.xml")).Returns(new Regex(@".*\.xml$"));
            globToRegex.Setup(g => g.ConvertToRegex("include.xml")).Returns(new Regex(@"include\.xml$"));
            globToRegex.Setup(g => g.ConvertToRegex(@"_svn\")).Returns(new Regex(@"_svn\\.*$"));

            var excluder = new FileNameFilter(globToRegex.Object, new [] {"*.xml", @"_svn\"}, new [] {"include.xml"});
            Assert.That(excluder.IncludeFile("test.xml"), Is.EqualTo(false));
            Assert.That(excluder.IncludeFile("include.xml"), Is.EqualTo(true));
            Assert.That(excluder.IncludeFile("test.txt"), Is.EqualTo(true));
            Assert.That(excluder.IncludeFile(@"_svn\test.txt"), Is.EqualTo(false));
        }
    }
}