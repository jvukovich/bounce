using System;
using System.IO;
using Bounce.Framework;
using NUnit.Framework;

namespace Bounce.Tests {
    [TestFixture]
    public class ToDirTest {
        [SetUp]
        public void SetUp() {
            RecreateDirectory("testfrom");
            RecreateDirectory("testto");
        }

        private void RecreateDirectory(string dir) {
            if (Directory.Exists(dir)) {
                Directory.Delete(dir, true);
            }
            Directory.CreateDirectory(dir);
        }

        [Test]
        public void ShouldCopyContents() {
            var toDir = new ToDir {FromPath = "testfrom".V(), ToPath = "testto".V()};

            Touch(@"testfrom\one.txt", "one");
            Touch(@"testfrom\two.txt", "two");
            Touch(@"testfrom\subdir\three.txt", "three");

            toDir.Build();

            AssertFileContains(@"testto\one.txt", "one");
            AssertFileContains(@"testto\two.txt", "two");
            AssertFileContains(@"testto\subdir\three.txt", "three");
        }

        [Test]
        public void ShouldDeleteToDirectoryWhenCleaning() {
            Assert.That(Directory.Exists("testto"));

            Touch(@"testto\one.txt", "one");

            var toDir = new ToDir {FromPath = "testfrom".V(), ToPath = "testto".V()};

            toDir.Clean();

            Assert.That(!Directory.Exists("testto"));
        }

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
    }
}