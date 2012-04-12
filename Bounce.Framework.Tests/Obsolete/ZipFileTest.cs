using System;
using Bounce.Framework.Obsolete;
using Moq;
using NUnit.Framework;

namespace Bounce.Framework.Tests.Obsolete {
    [TestFixture]
    public class ZipFileTest {
        [Test]
        public void ShouldCreateZipFileIfDoesntExist() {
            var fs = new Mock<IFileUtils>();
            var zipCreator = new Mock<IZipFileCreator>();
            var directoryUtils = new Mock<IDirectoryUtils>();

            string zipfilename = "zipfile";
            fs.Setup(f => f.FileExists(zipfilename)).Returns(false);

            string directoryToBeZipped = "dir";

            var zip = new ZipFile(zipCreator.Object, fs.Object, directoryUtils.Object) {ZipFileName = zipfilename, Directory = directoryToBeZipped};

            zip.Build();

            zipCreator.Verify(z => z.CreateZipFile(zipfilename, directoryToBeZipped), Times.Once());
        }

        [Test]
        public void ShouldNotCreateZipFileIfExistsAndModifiedLaterThanDirectory() {
            var fs = new Mock<IFileUtils>();
            var directoryUtils = new Mock<IDirectoryUtils>();
            var zipCreator = new Mock<IZipFileCreator>();

            string zipfilename = "zipfile";
            string directoryToBeZipped = "dir";

            fs.Setup(f => f.FileExists(zipfilename)).Returns(true);
            var directoryModTime = new DateTime(2010, 5, 4);

            fs.Setup(f => f.LastWriteTimeForFile(zipfilename)).Returns(directoryModTime.AddDays(1));
            directoryUtils.Setup(d => d.GetLastModTimeForDirectory(directoryToBeZipped)).Returns(directoryModTime);

            var zip = new ZipFile(zipCreator.Object, fs.Object, directoryUtils.Object) {ZipFileName = zipfilename, Directory = directoryToBeZipped};

            zip.Build();

            zipCreator.Verify(z => z.CreateZipFile(zipfilename, directoryToBeZipped), Times.Never());
        }

        [Test]
        public void ShouldCreateZipFileIfExistsAndModifiedBeforeDirectory() {
            var fs = new Mock<IFileUtils>();
            var directoryUtils = new Mock<IDirectoryUtils>();
            var zipCreator = new Mock<IZipFileCreator>();

            string zipfilename = "zipfile";
            string directoryToBeZipped = "dir";

            fs.Setup(f => f.FileExists(zipfilename)).Returns(true);
            var directoryModTime = new DateTime(2010, 5, 4);

            fs.Setup(f => f.LastWriteTimeForFile(zipfilename)).Returns(directoryModTime.AddDays(-1));
            directoryUtils.Setup(d => d.GetLastModTimeForDirectory(directoryToBeZipped)).Returns(directoryModTime);

            var zip = new ZipFile(zipCreator.Object, fs.Object, directoryUtils.Object) {ZipFileName = zipfilename, Directory = directoryToBeZipped};

            zip.Build();

            fs.Verify(f => f.DeleteFile(zipfilename), Times.Once());
            zipCreator.Verify(z => z.CreateZipFile(zipfilename, directoryToBeZipped), Times.Once());
        }
    }
}