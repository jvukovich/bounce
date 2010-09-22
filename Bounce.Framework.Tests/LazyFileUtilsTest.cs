using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;

namespace Bounce.Framework.Tests {
    [TestFixture]
    public class LazyFileUtilsTest {
        [Test]
        public void ShouldCopyIfDestDoesntExist() {
            var fs = new Mock<IFileUtils>();
            string src = "src";
            string dest = "dest";

            fs.Setup(d => d.FileExists(dest)).Returns(false);

            var lazy = new LazyFileUtils(fs.Object);
            lazy.CopyFile(src, dest);

            fs.Verify(d => d.CopyFile(src, dest), Times.Once());
        }

        [Test]
        public void ShouldCopyIfDestExistsButIsOlderThanSrc() {
            var fs = new Mock<IFileUtils>();
            string src = "src";
            string dest = "dest";

            fs.Setup(d => d.FileExists(dest)).Returns(true);
            var destModTime = new DateTime(2010, 3, 1);
            fs.Setup(d => d.LastWriteTimeForFile(dest)).Returns(destModTime);
            fs.Setup(d => d.LastWriteTimeForFile(src)).Returns(destModTime.AddDays(1));

            var lazy = new LazyFileUtils(fs.Object);
            lazy.CopyFile(src, dest);

            fs.Verify(d => d.CopyFile(src, dest), Times.Once());
        }

        [Test]
        public void ShouldNotCopyIfDestExistsAndIsNewerThanSrc() {
            var fs = new Mock<IFileUtils>();
            string src = "src";
            string dest = "dest";

            fs.Setup(d => d.FileExists(dest)).Returns(true);
            var destModTime = new DateTime(2010, 3, 1);
            fs.Setup(d => d.LastWriteTimeForFile(dest)).Returns(destModTime);
            fs.Setup(d => d.LastWriteTimeForFile(src)).Returns(destModTime.AddDays(-1));

            var lazy = new LazyFileUtils(fs.Object);
            lazy.CopyFile(src, dest);

            fs.Verify(d => d.CopyFile(src, dest), Times.Never());
        }
    }
}