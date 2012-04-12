using System;
using System.IO;
using Bounce.Framework.Obsolete;
using Moq;
using NUnit.Framework;

namespace Bounce.Framework.Tests.Obsolete
{
    [TestFixture]
    public class RenameFileTests
    {
        [Test]
        public void ShouldRenameFile()
        {
            var mockBounce = new Mock<IBounce>();

            var from = Path.GetTempFileName();
            var to = Path.GetTempFileName();
            File.Delete(to); //GetTempFileName() creates the file

            Assert.That(File.Exists(from), Is.True, "File " + from + " should exist");
            Assert.That(File.Exists(to), Is.False, "File " + to + " should not exist");

            var rename = new RenameFile {From = from, To = to};
            rename.Build(mockBounce.Object);

            Assert.That(File.Exists(from), Is.False, "File " + from + " should not exist");
            Assert.That(File.Exists(to), Is.True, "File " + to + " should exist");
            File.Delete(to);
        }

        [Test]
        public void ShouldThrowExceptionWhenTargetAlreadyExists()
        {
            var mockBounce = new Mock<IBounce>();

            var from = Path.GetTempFileName(); //GetTempFileName() creates the file
            var to = Path.GetTempFileName();

            Assert.That(File.Exists(from), Is.True, "File " + from + " should exist");
            Assert.That(File.Exists(to), Is.True, "File " + to + " should exist");

            var rename = new RenameFile { From = from, To = to };
            Assert.Throws<ArgumentException>(() => rename.Build(mockBounce.Object));

            Assert.That(File.Exists(from), Is.True, "File " + from + " should exist");
            Assert.That(File.Exists(to), Is.True, "File " + to + " should exist");
            File.Delete(from);
            File.Delete(to);
        }

        [Test]
        public void ShouldThrowExceptionWhenSourceDoesntExist()
        {
            var mockBounce = new Mock<IBounce>();

            var from = Path.GetTempFileName(); //GetTempFileName() creates the file
            var to = Path.GetTempFileName();
            File.Delete(from);
            File.Delete(to);

            Assert.That(File.Exists(from), Is.False, "File " + from + " should not exist");
            Assert.That(File.Exists(to), Is.False, "File " + to + " should not exist");

            var rename = new RenameFile { From = from, To = to };
            Assert.Throws<ArgumentException>(() => rename.Build(mockBounce.Object));
        }
    }
}