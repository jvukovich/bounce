using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;

namespace Bounce.Framework.Tests {
    [TestFixture]
    public class CopyTest {
        [Test]
        public void ShouldCopyIfFromPathNewer() {
            var copier = new Mock<IFileSystemCopier>();
            var fromPath = "fromdir";
            var toPath = "todir";

            copier.Setup(c => c.Exists(toPath)).Returns(true);
            copier.Setup(c => c.Exists(fromPath)).Returns(true);
            var fromPathLastModified = new DateTime(2010, 5, 10);
            copier.Setup(c => c.GetLastModTimeForPath(fromPath)).Returns(fromPathLastModified);
            copier.Setup(c => c.GetLastModTimeForPath(toPath)).Returns(fromPathLastModified.AddDays(-1));

            var includes = new string[0];
            var excludes = new string[0];
            var toDir = new Copy(copier.Object) {ToPath = toPath, FromPath = fromPath, Includes = includes, Excludes = excludes};

            toDir.Build();

            copier.Verify(c => c.Copy(fromPath, toPath, excludes, includes), Times.Once());
        }

        [Test]
        public void ShouldCopyIfToPathDoesntExist() {
            var copier = new Mock<IFileSystemCopier>();
            var fromPath = "fromdir";
            var toPath = "todir";

            copier.Setup(c => c.Exists(toPath)).Returns(false);
            copier.Setup(c => c.Exists(fromPath)).Returns(true);

            var includes = new string[0];
            var excludes = new string[0];
            var toDir = new Copy(copier.Object) {ToPath = toPath, FromPath = fromPath, Includes = includes, Excludes = excludes};

            toDir.Build();

            copier.Verify(c => c.GetLastModTimeForPath(toPath), Times.Never());
            copier.Verify(c => c.GetLastModTimeForPath(fromPath), Times.Never());
            copier.Verify(c => c.Copy(fromPath, toPath, excludes, includes), Times.Once());
        }

        [Test]
        public void ShouldNotCopyIfFromPathOlder() {
            var copier = new Mock<IFileSystemCopier>();
            var fromPath = "fromdir";
            var toPath = "todir";

            copier.Setup(c => c.Exists(toPath)).Returns(true);
            copier.Setup(c => c.Exists(fromPath)).Returns(true);

            var fromPathLastModified = new DateTime(2010, 5, 10);
            copier.Setup(c => c.GetLastModTimeForPath(fromPath)).Returns(fromPathLastModified);
            copier.Setup(c => c.GetLastModTimeForPath(toPath)).Returns(fromPathLastModified.AddDays(1));

            var toDir = new Copy(copier.Object) {ToPath = toPath, FromPath = fromPath};

            toDir.Build();

            copier.Verify(c => c.Copy(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>()), Times.Never());
        }

        [Test]
        public void ShouldDeleteDirectory() {
            var copier = new Mock<IFileSystemCopier>();
            var toPath = "todir";

            var toDir = new Copy(copier.Object) { ToPath = toPath};

            toDir.Clean();

            copier.Verify(c => c.Delete(toPath), Times.Once());
        }
    }
}