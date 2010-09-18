using Bounce.Framework;
using Moq;
using NUnit.Framework;

namespace Bounce.Tests {
    [TestFixture]
    public class GitRepoTest {
        [Test]
        public void IfDirectoryAlreadyExtantShouldUsePull() {
            var git = new Mock<IGitCommand>();
            var dirs = new Mock<IDirectoryUtils>();
            var parser = new Mock<IGitRepoParser>();

            parser.Setup(p => p.ParseCloneDirectoryFromRepoUri("repo")).Returns("dir");
            dirs.Setup(d => d.DirectoryExists("dir")).Returns(true);

            var gitRepo = new GitRepo(parser.Object, dirs.Object, git.Object) { Origin = "repo".V() };
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

            var gitRepo = new GitRepo(parser.Object, dirs.Object, git.Object) {Origin = "repo".V()};
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

            var gitRepo = new GitRepo(parser.Object, dirs.Object, git.Object) {Origin = "repo".V(), Path = "path".V()};
            gitRepo.Build();

            git.Verify(g => g.Clone("repo", "path"), Times.Once());
        }

        [Test]
        public void CleanShouldDeleteDirectory() {
            var git = new Mock<IGitCommand>();
            var dirs = new Mock<IDirectoryUtils>();
            var parser = new Mock<IGitRepoParser>();

            var gitRepo = new GitRepo(parser.Object, dirs.Object, git.Object) {Origin = "repo".V(), Path = "path".V()};

            gitRepo.Clean();

            dirs.Verify(d => d.DeleteDirectory("path"), Times.Once());
        }
    }
}