using Moq;
using NUnit.Framework;

namespace Bounce.Framework.Tests {
    [TestFixture]
    public class GitCheckoutTest {
        [Test]
        public void IfDirectoryAlreadyExtantShouldUsePull() {
            var git = new Mock<IGitCommand>();
            var dirs = new Mock<IDirectoryUtils>();
            var parser = new Mock<IGitRepoParser>();

            parser.Setup(p => p.ParseCloneDirectoryFromRepoUri("repo")).Returns("dir");
            dirs.Setup(d => d.DirectoryExists("dir")).Returns(true);

            var gitRepo = new GitCheckout(parser.Object, dirs.Object, git.Object) { Repository = "repo" };
            gitRepo.Build();

            git.Verify(g => g.Pull(), Times.Once());
        }

        [Test]
        public void IfDirectoryNotExtantShouldUseClone() {
            var git = new Mock<IGitCommand>();
            var dirs = new Mock<IDirectoryUtils>();
            var parser = new Mock<IGitRepoParser>();

            parser.Setup(p => p.ParseCloneDirectoryFromRepoUri("repo")).Returns("dir");
            dirs.Setup(d => d.DirectoryExists("dir")).Returns(false);

            var gitRepo = new GitCheckout(parser.Object, dirs.Object, git.Object) {Repository = "repo"};
            gitRepo.Build();

            git.Verify(g => g.Clone("repo", "dir"), Times.Once());
        }

        [Test]
        public void IfPathGivenShouldBeUsedInClone() {
            var git = new Mock<IGitCommand>();
            var dirs = new Mock<IDirectoryUtils>();
            var parser = new Mock<IGitRepoParser>();

            parser.Setup(p => p.ParseCloneDirectoryFromRepoUri("repo")).Returns("dir");
            dirs.Setup(d => d.DirectoryExists("dir")).Returns(false);

            var gitRepo = new GitCheckout(parser.Object, dirs.Object, git.Object) {Repository = "repo", Directory = "path"};
            gitRepo.Build();

            git.Verify(g => g.Clone("repo", "path"), Times.Once());
        }

        [Test]
        public void CleanShouldDeleteDirectory() {
            var git = new Mock<IGitCommand>();
            var dirs = new Mock<IDirectoryUtils>();
            var parser = new Mock<IGitRepoParser>();

            var gitRepo = new GitCheckout(parser.Object, dirs.Object, git.Object) {Repository = "repo", Directory = "path"};

            gitRepo.Clean();

            dirs.Verify(d => d.DeleteDirectory("path"), Times.Once());
        }

        [Test]
        public void ShouldReturnPathsRelativeToWorkingDirectory() {
            var gitRepo = new GitCheckout() {Directory = "dir"};

            var subPath = gitRepo.Files["test.txt"];
            Assert.That(subPath.Value, Is.EqualTo(@"dir\test.txt"));
            Assert.That(subPath.Dependencies, Has.Member(gitRepo));
        }
    }
}