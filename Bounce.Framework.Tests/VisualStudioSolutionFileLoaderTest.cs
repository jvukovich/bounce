using System.Linq;
using NUnit.Framework;

namespace Bounce.Framework.Tests {
    [TestFixture]
    public class VisualStudioSolutionFileLoaderTest {
        [Test]
        public void ShouldReadProjectsFromSolutionFile()
        {
            string path = @"TestFiles\Bounce.sln";

            var reader = new VisualStudioSolutionFileLoader();
            VisualStudioSolutionFileDetails details = reader.LoadVisualStudioSolution(path);

            VisualStudioSolutionProjectReference bounce = details.VisualStudioProjects.ElementAt(0);
            Assert.That(bounce.Name, Is.EqualTo("Bounce"));
            Assert.That(bounce.Path, Is.EqualTo(@"Bounce\Bounce.csproj"));

            VisualStudioSolutionProjectReference bouncerConsole = details.VisualStudioProjects.ElementAt(1);
            Assert.That(bouncerConsole.Name, Is.EqualTo("Bouncer.Console"));
            Assert.That(bouncerConsole.Path, Is.EqualTo(@"Bouncer.Console\Bouncer.Console.csproj"));
        }
    }
}
