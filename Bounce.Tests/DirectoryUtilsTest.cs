using System;
using System.IO;
using Bounce.Framework;
using NUnit.Framework;

namespace Bounce.Tests {
    [TestFixture]
    public class DirectoryUtilsTest {
        private const string dir = @"testdir";

        [SetUp]
        public void SetUp() {
            if (Directory.Exists(dir)) {
                Directory.Delete(dir, true);
            }
            Directory.CreateDirectory(dir);

            Directory.SetLastWriteTimeUtc(dir, new DateTime(2010, 5, 1));
        }

        [Test]
        public void LastBuiltShouldReturnLastModDateOfFileInDirOrSubDir() {
            Touch("j.txt", new DateTime(2010, 5, 6));
            Touch(@"test\j.txt", new DateTime(2010, 5, 20));

            Assert.That(DirectoryUtils.GetLastModTimeForFilesInDirectory(dir), Is.EqualTo(new DateTime(2010, 5, 20)));
        }

        [Test]
        public void LastBuiltShouldReturnLastModDateOfDirectoryIfNoFiles() {
            Assert.That(DirectoryUtils.GetLastModTimeForFilesInDirectory(dir), Is.EqualTo(new DateTime(2010, 5, 1)));
        }

        private void Touch(string filename, DateTime modTime) {
            var fullPath = Path.Combine(dir, filename);
            var directoryName = Path.GetDirectoryName(fullPath);
            if (!Directory.Exists(directoryName)) {
                Directory.CreateDirectory(directoryName);
                var parentDir = Path.GetDirectoryName(directoryName);
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
    }
}