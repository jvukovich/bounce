using NUnit.Framework;

namespace Bounce.VisualStudio.Tests {
    [TestFixture]
    public class VisualStudioCSharpProjectFileLoaderTest {
        [Test]
        public void ShouldLoadLibraryFile () {
            var loader = new VisualStudioCSharpProjectFileLoader();
            string path = @"TestFiles\Bounce.csproj";
            var details = loader.LoadProject(path, "name", "");

            Assert.That(details.OutputFile, Is.EqualTo(@"bin\Debug\Bounce.dll"));
            Assert.That(details.Name, Is.EqualTo(@"name"));
        }

        [Test]
        public void ShouldLoadLibraryFileWithConfiguration () {
            var loader = new VisualStudioCSharpProjectFileLoader();
            string path = @"TestFiles\Bounce.csproj";
            var details = loader.LoadProject(path, "name", "Release");

            Assert.That(details.OutputFile, Is.EqualTo(@"bin\Release\Bounce.dll"));
            Assert.That(details.Name, Is.EqualTo(@"name"));
        }
    }
}