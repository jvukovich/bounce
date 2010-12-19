using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;

namespace Bounce.Framework.Tests {
    [TestFixture]
    public class CopyTest {
        [Test]
        public void ShouldCopy() {
            var copier = new Mock<IFileSystemCopier>();
            var fromPath = "fromdir";
            var toPath = "todir";

            var includes = new string[0];
            var excludes = new string[0];
            var toDir = new Copy(copier.Object) {ToPath = toPath, FromPath = fromPath, Includes = includes, Excludes = excludes};

            var bounce = new Mock<IBounce>();
            bounce.SetupGet(b => b.Log).Returns(new Mock<ILog>().Object);

            toDir.TestBuild();

            copier.Verify(c => c.Copy(fromPath, toPath, excludes, includes), Times.Once());
        }

        [Test]
        public void ShouldDeleteDirectory() {
            var copier = new Mock<IFileSystemCopier>();
            var toPath = "todir";

            var toDir = new Copy(copier.Object) { ToPath = toPath};

            toDir.TestClean();

            copier.Verify(c => c.Delete(toPath), Times.Once());
        }

        [Test]
        public void TakingDependencyOnToPathShouldTakeDependencyOnCopyItself()
        {
            Future<string> fromPath = "fromPath";
            Future<string> toPath = "toPath";

            var copy = new Copy {FromPath = fromPath, ToPath = toPath};

            Assert.That(copy.Dependencies, Has.Member(fromPath));
            Assert.That(copy.Dependencies, Has.Member(toPath));

            Assert.That(copy.ToPath.Dependencies, Has.Member(copy));
        }
    }
}