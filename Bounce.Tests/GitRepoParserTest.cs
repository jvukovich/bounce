using Bounce.Framework;
using NUnit.Framework;

namespace Bounce.Tests {
    [TestFixture]
    public class GitRepoParserTest {
        private GitRepoParser Parser;

        [SetUp]
        public void SetUp() {
            Parser = new GitRepoParser();
        }


        [Test]
        public void ShouldReturnLastComponentInPathWithoutGitExtension() {
            AssertParseDir("/path/to/repo.git", "repo");
            AssertParseDir("/path/to/repo/.git", "repo");
            AssertParseDir("/path/to/repo", "repo");
            AssertParseDir("/path/to/repo.git/", "repo");
            AssertParseDir(@"c:\path\to\repo.git\", "repo");
            AssertParseDir("ssh://user@host.xz:123/path/to/repo", "repo");
        }

        private void AssertParseDir(string repo, string dir) {
            Assert.That(Parser.ParseCloneDirectoryFromRepoUri(repo), Is.EqualTo(dir));
        }
    }
}