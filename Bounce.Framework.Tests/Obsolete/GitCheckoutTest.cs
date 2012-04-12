using System.Collections.Generic;
using Bounce.Framework.Obsolete;
using Moq;
using NUnit.Framework;

namespace Bounce.Framework.Tests.Obsolete {
    [TestFixture]
    public class GitCheckoutTest {
        private Mock<IBounce> Bounce;
        private ILog Log;

        [SetUp]
        public void SetUp() {
            Bounce = new Mock<IBounce>();
            Log = new Mock<ILog>().Object;
            Bounce.Setup(b => b.Log).Returns(Log);
        }

        [Test]
        public void IfDirectoryAlreadyExtantShouldUsePull() {
            var git = new Mock<IGitCommand>();
            var dirs = new Mock<IDirectoryUtils>();
            var parser = new Mock<IGitRepoParser>();

            parser.Setup(p => p.ParseCloneDirectoryFromRepoUri("repo")).Returns("dir");
            dirs.Setup(d => d.DirectoryExists("dir")).Returns(true);

            var gitRepo = new GitCheckout(parser.Object, dirs.Object, git.Object) { Repository = "repo" };

            gitRepo.Build(Bounce.Object);

            git.Verify(g => g.Pull("dir", Log, Bounce.Object), Times.Once());
        }

        [Test]
        public void IfDirectoryNotExtantShouldUseClone() {
            var git = new Mock<IGitCommand>();
            var dirs = new Mock<IDirectoryUtils>();
            var parser = new Mock<IGitRepoParser>();

            parser.Setup(p => p.ParseCloneDirectoryFromRepoUri("repo")).Returns("dir");
            dirs.Setup(d => d.DirectoryExists("dir")).Returns(false);

            var gitRepo = new GitCheckout(parser.Object, dirs.Object, git.Object) {Repository = "repo"};
            gitRepo.Build(Bounce.Object);

            git.Verify(g => g.Clone("repo", "dir", null, Log, Bounce.Object), Times.Once());
        }

        [Test]
        public void IfPathGivenShouldBeUsedInClone() {
            var git = new Mock<IGitCommand>();
            var dirs = new Mock<IDirectoryUtils>();
            var parser = new Mock<IGitRepoParser>();

            parser.Setup(p => p.ParseCloneDirectoryFromRepoUri("repo")).Returns("dir");
            dirs.Setup(d => d.DirectoryExists("dir")).Returns(false);

            var gitRepo = new GitCheckout(parser.Object, dirs.Object, git.Object) {Repository = "repo", Directory = "path"};
            gitRepo.Build(Bounce.Object);

            git.Verify(g => g.Clone("repo", "path", null, Log, Bounce.Object), Times.Once());
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
            Assert.That(subPath.IsDependentOn(gitRepo));
        }

        [Test]
        public void IfBranchIsProvidedShouldCloneWithBranchOptions() {
            var git = new Mock<IGitCommand>();
            var dirs = new Mock<IDirectoryUtils>();
            var parser = new Mock<IGitRepoParser>();

            parser.Setup(p => p.ParseCloneDirectoryFromRepoUri("repo")).Returns("dir");
            dirs.Setup(d => d.DirectoryExists("dir")).Returns(false);

            var gitRepo = new GitCheckout(parser.Object, dirs.Object, git.Object) { Repository = "repo", Directory = "path", Branch = "develop" };
            gitRepo.Build(Bounce.Object);

            var expectedOption = new Dictionary<string, string> { { "--branch", "develop" } };
            git.Verify(g => g.Clone("repo", "path", expectedOption, Log, Bounce.Object), Times.Once());
        }
    }
}