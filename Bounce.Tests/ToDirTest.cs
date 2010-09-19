using System;
using System.Collections.Generic;
using Bounce.Framework;
using Moq;
using NUnit.Framework;

namespace Bounce.Tests {
    [TestFixture]
    public class ToDirTest {
        [Test]
        public void ShouldCopyIfFromDirectoryIfNewer() {
            var dirUtils = new Mock<IDirectoryUtils>();
            var fromPath = "fromdir";
            var toPath = "todir";

            var fromPathLastModified = new DateTime(2010, 5, 10);
            dirUtils.Setup(d => d.GetLastModTimeForDirectory(fromPath)).Returns(fromPathLastModified);
            dirUtils.Setup(d => d.GetLastModTimeForDirectory(toPath)).Returns(fromPathLastModified.AddDays(-1));

            var includes = new string[0];
            var excludes = new string[0];
            var toDir = new ToDir(dirUtils.Object) {ToPath = toPath, FromPath = fromPath, Includes = includes, Excludes = excludes};

            toDir.Build();

            dirUtils.Verify(d => d.CopyDirectoryContents(fromPath, toPath, excludes, includes), Times.Once());
        }

        [Test]
        public void ShouldNotCopyIfFromDirectoryOlder() {
            var dirUtils = new Mock<IDirectoryUtils>();
            var fromPath = "fromdir";
            var toPath = "todir";

            var fromPathLastModified = new DateTime(2010, 5, 10);
            dirUtils.Setup(d => d.GetLastModTimeForDirectory(fromPath)).Returns(fromPathLastModified);
            dirUtils.Setup(d => d.GetLastModTimeForDirectory(toPath)).Returns(fromPathLastModified.AddDays(1));

            var toDir = new ToDir(dirUtils.Object) {ToPath = toPath, FromPath = fromPath};

            toDir.Build();

            dirUtils.Verify(d => d.CopyDirectoryContents(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>()), Times.Never());
        }

        [Test]
        public void ShouldDeleteDirectory() {
            var dirUtils = new Mock<IDirectoryUtils>();
            var toPath = "todir";

            var toDir = new ToDir(dirUtils.Object) { ToPath = toPath};

            toDir.Clean();

            dirUtils.Verify(d => d.DeleteDirectory(toPath), Times.Once());
        }
    }
}