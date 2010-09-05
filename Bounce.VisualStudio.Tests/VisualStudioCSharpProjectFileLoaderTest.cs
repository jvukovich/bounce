using NUnit.Framework;

namespace Bounce.VisualStudio.Tests {
    [TestFixture]
    public class VisualStudioCSharpProjectFileLoaderTest {
        [Test]
        public void ShouldLoadLibraryFile () {
            var loader = new VisualStudioCSharpProjectFileLoader();
            string path = @"TestFiles\Bounce.csproj";
            var details = loader.LoadProject(path, "");

            Assert.That(details.OutputFile, Is.EqualTo(@"bin\Debug\Bounce.dll"));
        }

        [Test]
        public void ShouldLoadLibraryFileWithConfiguration () {
            var loader = new VisualStudioCSharpProjectFileLoader();
            string path = @"TestFiles\Bounce.csproj";
            var details = loader.LoadProject(path, "Release");

            Assert.That(details.OutputFile, Is.EqualTo(@"bin\Release\Bounce.dll"));
        }
    }
}