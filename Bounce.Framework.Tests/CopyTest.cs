using System;
using System.Collections.Generic;
using System.Linq;
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
            Task<string> fromPath = "fromPath";
            Task<string> toPath = "toPath";

            var copy = new Copy {FromPath = fromPath, ToPath = toPath};

            Assert.That(copy.Dependencies.Select(d => d.Task), Has.Member(fromPath));
            Assert.That(copy.Dependencies.Select(d => d.Task), Has.Member(toPath));

            Assert.That(copy.ToPath.Dependencies.Select(d => d.Task), Has.Member(copy));
        }
    }
}