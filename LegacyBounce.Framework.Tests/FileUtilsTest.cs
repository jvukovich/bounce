using System;
using System.IO;
using NUnit.Framework;

namespace LegacyBounce.Framework.Tests {
    [TestFixture]
    public class FileUtilsTest {
        [Test]
        public void ExistsShouldReturnTrueIfFileExists() {
            RecreateDirectory("test");

            Touch(@"test\test.txt");

            var fu = new FileUtils();
            Assert.That(fu.FileExists(@"test\test.txt"));
            Assert.That(fu.FileExists(@"test\nothere.txt"), Is.False);
        }

        [Test]
        public void LastModTimeShouldReturnLastModTime() {
            RecreateDirectory("test");

            var lastModTime = new DateTime(2010, 5, 20);
            Touch(@"test\test.txt", lastModTime);

            var fu = new FileUtils();
            Assert.That(fu.LastWriteTimeForFile(@"test\test.txt"), Is.EqualTo(lastModTime));
        }

        [Test]
        public void DeleteShouldDeleteFile() {
            RecreateDirectory("test");

            Touch(@"test\test.txt");

            var fu = new FileUtils();
            fu.DeleteFile(@"test\test.txt");

            Assert.That(File.Exists(@"test\test.txt"), Is.False);
        }

        [Test]
        public void ShouldCopyFile() {
            RecreateDirectory("test");

            File.WriteAllText(@"test\test.txt", "sometext");

            var fu = new FileUtils();
            fu.CopyFile(@"test\test.txt", @"test\other.txt");

            Assert.That(File.ReadAllText(@"test\other.txt"), Is.EqualTo("sometext"));
        }

        private void Touch(string file) {
            File.WriteAllText(file, "");
        }

        private void Touch(string file, DateTime lastModTime) {
            File.WriteAllText(file, "");
            File.SetLastWriteTimeUtc(file, lastModTime);
        }

        private void RecreateDirectory(string d) {
            if (Directory.Exists(d)) {
                Directory.Delete(d, true);
            }
            Directory.CreateDirectory(d);
        }
    }
}