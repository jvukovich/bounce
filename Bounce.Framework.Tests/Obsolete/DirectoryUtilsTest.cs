using System;
using System.IO;
using Bounce.Framework.Obsolete;
using Bounce.TestHelpers;
using Moq;
using NUnit.Framework;

namespace Bounce.Framework.Tests.Obsolete {
    [TestFixture]
    public class DirectoryUtilsTest {
        [SetUp]
        public void SetUp() {
            if (Directory.Exists(Dir)) {
                Directory.Delete(Dir, true);
            }
            Directory.CreateDirectory(Dir);

            Directory.SetLastWriteTimeUtc(Dir, new DateTime(2010, 5, 1));
        }

        private const string Dir = @"testdir";

        private void AssertFileContains(string filename, string contents) {
            Assert.That(File.Exists(filename), String.Format("file `{0}' should exist", filename));
            Assert.That(File.ReadAllText(filename), Is.EqualTo(contents));
        }

        private void Touch(string filename, string contents) {
            CreatePath(Path.GetDirectoryName(filename));
            File.WriteAllText(filename, contents);
        }

        private void CreatePath(string dir) {
            if (string.IsNullOrEmpty(dir)) {
                return;
            }

            CreatePath(Path.GetDirectoryName(dir));

            if (!Directory.Exists(dir)) {
                Directory.CreateDirectory(dir);
            }
        }

        private void Touch(string filename, DateTime modTime) {
            string fullPath = Path.Combine(Dir, filename);
            string directoryName = Path.GetDirectoryName(fullPath);
            if (!Directory.Exists(directoryName)) {
                Directory.CreateDirectory(directoryName);
                string parentDir = Path.GetDirectoryName(directoryName);
                if (!string.IsNullOrEmpty(parentDir)) {
                    Directory.SetLastWriteTimeUtc(parentDir, new DateTime(2010, 5, 1));
                }
            }
            File.WriteAllText(fullPath, "");
            File.SetLastWriteTimeUtc(fullPath, modTime);

            // the directory write time is updated to now whenever a new file is created.
            // lets set it back, so to pretend that the files were modified, not created.
            Directory.SetLastWriteTimeUtc(directoryName, new DateTime(2010, 5, 1));
        }

        [Test]
        public void LastModTimeShouldReturnLastModDateOfDirectoryIfNoFiles() {
            var directoryUtils = new DirectoryUtils();
            Assert.That(directoryUtils.GetLastModTimeForDirectory(Dir), Is.EqualTo(new DateTime(2010, 5, 1)));
        }

        [Test]
        public void LastModTimeShouldReturnLastModDateOfFileInDirOrSubDir() {
            Touch("j.txt", new DateTime(2010, 5, 6));
            Touch(@"test\j.txt", new DateTime(2010, 5, 20));

            var directoryUtils = new DirectoryUtils();
            Assert.That(directoryUtils.GetLastModTimeForDirectory(Dir), Is.EqualTo(new DateTime(2010, 5, 20)));
        }

        [Test]
        public void ShouldCopyContents() {
            FileSystemTestHelper.RecreateDirectory("testfrom");
            FileSystemTestHelper.RecreateDirectory("testto");

            Touch(@"testfrom\one.txt", "one");
            Touch(@"testfrom\two.txt", "two");
            Touch(@"testfrom\subdir\three.txt", "three");
            Touch(@"testfrom\subdir\exclude.txt", "three");

            var includes = new string[0];
            var excludes = new string[0];

            var filterFactory = new Mock<IFileNameFilterFactory>();
            var filter = new Mock<IFileNameFilter>();
            filterFactory.Setup(ff => ff.CreateFileNameFilter(excludes, includes)).Returns(filter.Object);

            filter.Setup(f => f.IncludeFile(@"one.txt")).Returns(true);
            filter.Setup(f => f.IncludeFile(@"two.txt")).Returns(true);
            filter.Setup(f => f.IncludeFile(@"subdir\three.txt")).Returns(true);
            filter.Setup(f => f.IncludeFile(@"subdir\exclude.txt")).Returns(false);
            filter.Setup(f => f.IncludeFile(@"subdir\")).Returns(true);

            new DirectoryUtils(filterFactory.Object).CopyDirectory("testfrom", "testto", excludes, includes);

            AssertFileContains(@"testto\one.txt", "one");
            AssertFileContains(@"testto\two.txt", "two");
            AssertFileContains(@"testto\subdir\three.txt", "three");
            Assert.That(File.Exists(@"testto\subdir\exclude.txt"), Is.False);
        }

        [Test]
        public void ShouldDeleteDirectory() {
            string dir = "test";
            FileSystemTestHelper.RecreateDirectory(dir);

            Assert.That(Directory.Exists(dir));

            Touch(@"test\one.txt", "one");

            var directoryUtils = new DirectoryUtils();

            directoryUtils.DeleteDirectory(dir);
            Assert.That(!Directory.Exists(dir));

            directoryUtils.DeleteDirectory(dir);
            Assert.That(!Directory.Exists(dir));
        }
    }
}