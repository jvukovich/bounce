using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;

namespace Bounce.Framework.Tests {
    [TestFixture]
    public class FileSystemCopierTest {
        [Test]
        public void IfSourceIsFileAndDestIsDirectoryThenCopyFileIntoDirectory() {
            var from = @"from\file";
            var to = @"to\dir";

            var files = new Mock<IFileUtils>();
            files.Setup(f => f.FileExists(from)).Returns(true);
            files.Setup(f => f.FileExists(to)).Returns(false);
            var dirs = new Mock<IDirectoryUtils>();
            dirs.Setup(d => d.DirectoryExists(from)).Returns(false);
            dirs.Setup(d => d.DirectoryExists(to)).Returns(true);

            new FileSystemCopier(files.Object, dirs.Object).Copy(from, to, null, null, true);

            files.Verify(f => f.CopyFile(from, @"to\dir\file"));
        }

        [Test]
        public void IfSourceIsFileAndDestIsFileThenCopyFile() {
            var from = @"from\file";
            var to = @"to\file";

            var files = new Mock<IFileUtils>();
            files.Setup(f => f.FileExists(from)).Returns(true);
            files.Setup(f => f.FileExists(to)).Returns(false);
            var dirs = new Mock<IDirectoryUtils>();
            dirs.Setup(d => d.DirectoryExists(from)).Returns(false);
            dirs.Setup(d => d.DirectoryExists(to)).Returns(false);

            new FileSystemCopier(files.Object, dirs.Object).Copy(from, to, null, null, true);

            files.Verify(f => f.CopyFile(from, @"to\file"));
        }

        [Test]
        public void IfSourceIsDirectoryThenCopyDirectory() {
            var from = @"from\dir";
            var to = @"to\dir";

            var files = new Mock<IFileUtils>();
            files.Setup(f => f.FileExists(from)).Returns(false);
            files.Setup(f => f.FileExists(to)).Returns(false);
            var dirs = new Mock<IDirectoryUtils>();
            dirs.Setup(d => d.DirectoryExists(from)).Returns(true);
            dirs.Setup(d => d.DirectoryExists(to)).Returns(true);

            IEnumerable<string> excludes = new string[0];
            IEnumerable<string> includes = new string[0];
            new FileSystemCopier(files.Object, dirs.Object).Copy(from, to, excludes, includes, true);

            dirs.Verify(d => d.DeleteDirectory(to), Times.Never());
            dirs.Verify(d => d.CopyDirectory(from, to, excludes, includes));
        }

        [Test]
        public void ShouldReturnLastModTimeOfDirectoryIfExists() {
            var dir = @"from\dir";

            var files = new Mock<IFileUtils>();
            files.Setup(f => f.FileExists(dir)).Returns(false);
            var dirs = new Mock<IDirectoryUtils>();
            dirs.Setup(d => d.DirectoryExists(dir)).Returns(true);

            var lastModTime = new DateTime(2010, 5, 10);
            dirs.Setup(d => d.GetLastModTimeForDirectory(dir)).Returns(lastModTime);

            var fileSystemCopier = new FileSystemCopier(files.Object, dirs.Object);
            Assert.That(fileSystemCopier.GetLastModTimeForPath(dir), Is.EqualTo(lastModTime));
        }

        [Test]
        public void ShouldReturnLastModTimeOfFileIfExists() {
            var file = @"from\file";

            var files = new Mock<IFileUtils>();
            files.Setup(f => f.FileExists(file)).Returns(true);
            var dirs = new Mock<IDirectoryUtils>();
            dirs.Setup(d => d.DirectoryExists(file)).Returns(false);

            var lastModTime = new DateTime(2010, 5, 10);
            files.Setup(f => f.LastWriteTimeForFile(file)).Returns(lastModTime);

            var fileSystemCopier = new FileSystemCopier(files.Object, dirs.Object);
            Assert.That(fileSystemCopier.GetLastModTimeForPath(file), Is.EqualTo(lastModTime));
        }

        [Test]
        public void ShouldDeleteDirectory() {
            var dir = @"from\dir";

            var files = new Mock<IFileUtils>();
            files.Setup(f => f.FileExists(dir)).Returns(false);
            var dirs = new Mock<IDirectoryUtils>();
            dirs.Setup(d => d.DirectoryExists(dir)).Returns(true);

            var fileSystemCopier = new FileSystemCopier(files.Object, dirs.Object);
            fileSystemCopier.Delete(dir);

            dirs.Verify(d => d.DeleteDirectory(dir), Times.Once());
        }

        [Test]
        public void ShouldDeleteFile() {
            var file = @"from\file";

            var files = new Mock<IFileUtils>();
            files.Setup(f => f.FileExists(file)).Returns(true);
            var dirs = new Mock<IDirectoryUtils>();
            dirs.Setup(d => d.DirectoryExists(file)).Returns(false);

            var fileSystemCopier = new FileSystemCopier(files.Object, dirs.Object);
            fileSystemCopier.Delete(file);

            files.Verify(f => f.DeleteFile(file), Times.Once());
        }

        [Test]
        public void FileShouldExist() {
            var file = @"from\file";

            var files = new Mock<IFileUtils>();
            files.Setup(f => f.FileExists(file)).Returns(true);
            var dirs = new Mock<IDirectoryUtils>();
            dirs.Setup(d => d.DirectoryExists(file)).Returns(false);

            var fileSystemCopier = new FileSystemCopier(files.Object, dirs.Object);
            Assert.That(fileSystemCopier.Exists(file));
        }

        [Test]
        public void DirectoryShouldExist() {
            var dir = @"from\dir";

            var files = new Mock<IFileUtils>();
            files.Setup(f => f.FileExists(dir)).Returns(false);
            var dirs = new Mock<IDirectoryUtils>();
            dirs.Setup(d => d.DirectoryExists(dir)).Returns(true);

            var fileSystemCopier = new FileSystemCopier(files.Object, dirs.Object);
            Assert.That(fileSystemCopier.Exists(dir));
        }

        [Test]
        public void FileOrDirectoryDoesntExist() {
            var dir = @"from\dir";

            var files = new Mock<IFileUtils>();
            files.Setup(f => f.FileExists(dir)).Returns(false);
            var dirs = new Mock<IDirectoryUtils>();
            dirs.Setup(d => d.DirectoryExists(dir)).Returns(false);

            var fileSystemCopier = new FileSystemCopier(files.Object, dirs.Object);
            Assert.That(fileSystemCopier.Exists(dir), Is.False);
        }
    }
}