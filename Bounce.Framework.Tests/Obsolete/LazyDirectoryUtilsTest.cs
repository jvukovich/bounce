using System;
using System.Collections.Generic;
using Bounce.Framework.Obsolete;
using Moq;
using NUnit.Framework;

namespace Bounce.Framework.Tests.Obsolete {
    [TestFixture]
    public class LazyDirectoryUtilsTest {
        [Test]
        public void ShouldCopyIfDestDoesntExist() {
            var du = new Mock<IDirectoryUtils>();
            string src = "src";
            string dest = "dest";

            du.Setup(d => d.DirectoryExists(dest)).Returns(false);

            IEnumerable<string> excludes = new string[0];
            IEnumerable<string> includes = new string[0];

            var lazy = new LazyDirectoryUtils(du.Object);
            lazy.CopyDirectory(src, dest, excludes, includes);

            du.Verify(d => d.CopyDirectory(src, dest, excludes, includes), Times.Once());
        }

        [Test]
        public void ShouldCopyIfDestExistsButIsOlderThanSrc() {
            var du = new Mock<IDirectoryUtils>();
            string src = "src";
            string dest = "dest";

            du.Setup(d => d.DirectoryExists(dest)).Returns(true);
            var destModTime = new DateTime(2010, 3, 1);
            du.Setup(d => d.GetLastModTimeForDirectory(dest)).Returns(destModTime);
            du.Setup(d => d.GetLastModTimeForDirectory(src)).Returns(destModTime.AddDays(1));

            IEnumerable<string> excludes = new string[0];
            IEnumerable<string> includes = new string[0];

            var lazy = new LazyDirectoryUtils(du.Object);
            lazy.CopyDirectory(src, dest, excludes, includes);

            du.Verify(d => d.CopyDirectory(src, dest, excludes, includes), Times.Once());
        }

        [Test]
        public void ShouldNotCopyIfDestExistsAndIsNewerThanSrc() {
            var du = new Mock<IDirectoryUtils>();
            string src = "src";
            string dest = "dest";

            du.Setup(d => d.DirectoryExists(dest)).Returns(true);
            var destModTime = new DateTime(2010, 3, 1);
            du.Setup(d => d.GetLastModTimeForDirectory(dest)).Returns(destModTime);
            du.Setup(d => d.GetLastModTimeForDirectory(src)).Returns(destModTime.AddDays(-1));

            IEnumerable<string> excludes = new string[0];
            IEnumerable<string> includes = new string[0];

            var lazy = new LazyDirectoryUtils(du.Object);
            lazy.CopyDirectory(src, dest, excludes, includes);

            du.Verify(d => d.CopyDirectory(src, dest, excludes, includes), Times.Never());
        }
    }
}