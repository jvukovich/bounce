using Bounce.Framework.Obsolete;
using NUnit.Framework;

namespace Bounce.Framework.Tests.Obsolete {
    [TestFixture]
    public class VisualStudioCSharpProjectFileLoaderTest {
        [Test]
        public void ShouldLoadLibraryFile () {
            var loader = new VisualStudioCSharpProjectFileLoader();
            string path = @"Obsolete\TestFiles\Bounce.csproj";
            var details = loader.LoadProject(path, "name", "");

            Assert.That(details.OutputDirectory, Is.EqualTo(@"Obsolete\TestFiles\bin\Debug"));
            Assert.That(details.OutputFile, Is.EqualTo(@"Obsolete\TestFiles\bin\Debug\Bounce.dll"));
            Assert.That(details.Name, Is.EqualTo(@"name"));
            Assert.That(details.ProjectFile, Is.EqualTo(@"Obsolete\TestFiles\Bounce.csproj"));
            Assert.That(details.ProjectDirectory, Is.EqualTo(@"Obsolete\TestFiles"));
        }

        [Test]
        public void ShouldLoadLibraryFileWithConfiguration () {
            var loader = new VisualStudioCSharpProjectFileLoader();
            string path = @"Obsolete\TestFiles\Bounce.csproj";
            var details = loader.LoadProject(path, "name", "Release");

            Assert.That(details.OutputDirectory, Is.EqualTo(@"Obsolete\TestFiles\bin\Release"));
            Assert.That(details.OutputFile, Is.EqualTo(@"Obsolete\TestFiles\bin\Release\Bounce.dll"));
            Assert.That(details.Name, Is.EqualTo(@"name"));
            Assert.That(details.ProjectFile, Is.EqualTo(@"Obsolete\TestFiles\Bounce.csproj"));
            Assert.That(details.ProjectDirectory, Is.EqualTo(@"Obsolete\TestFiles"));
        }
    }
}