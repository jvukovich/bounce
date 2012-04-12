using System;
using System.IO;
using Bounce.Framework.Obsolete;
using Moq;
using NUnit.Framework;

namespace Bounce.Framework.Tests.Obsolete
{
    [TestFixture]
    public class RenameDirectoryTests
    {
        [Test]
        public void ShouldRenameDirectory()
        {
            var mockBounce = new Mock<IBounce>();

            var from = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            var to = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(from);

            Assert.That(Directory.Exists(from), Is.True, "Directory " + from + " should exist");
            Assert.That(Directory.Exists(to), Is.False, "Directory " + to + " should not exist");

            var rename = new RenameDirectory { From = from, To = to };
            rename.Build(mockBounce.Object);

            Assert.That(Directory.Exists(from), Is.False, "Directory " + from + " should not exist");
            Assert.That(Directory.Exists(to), Is.True, "Directory " + to + " should exist");
            
            Directory.Delete(to);
        }


        [Test]
        public void ShouldThrowExceptionWhenTargetAlreadyExists()
        {
            var mockBounce = new Mock<IBounce>();

            var from = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            var to = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(from);
            Directory.CreateDirectory(to);

            Assert.That(Directory.Exists(from), Is.True, "Directory " + from + " should exist");
            Assert.That(Directory.Exists(to), Is.True, "Directory " + to + " should exist");

            var rename = new RenameDirectory { From = from, To = to };
            Assert.Throws<ArgumentException>(() => rename.Build(mockBounce.Object));

            Assert.That(Directory.Exists(from), Is.True, "Directory " + from + " should exist");
            Assert.That(Directory.Exists(to), Is.True, "Directory " + to + " should exist");
            Directory.Delete(from);
            Directory.Delete(to);
        }

        [Test]
        public void ShouldThrowExceptionWhenSourceDoesntExist()
        {
            var mockBounce = new Mock<IBounce>();

            var from = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            var to = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

            Assert.That(Directory.Exists(from), Is.False, "Directory " + from + " should not exist");
            Assert.That(Directory.Exists(to), Is.False, "Directory " + to + " should not exist");

            var rename = new RenameDirectory { From = from, To = to };
            Assert.Throws<ArgumentException>(() => rename.Build(mockBounce.Object));
        }
    }
}