using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;

namespace Bounce.VisualStudio.Tests {
    [TestFixture]
    public class VisualStudioSolutionFileReaderTest {
        [Test]
        public void ShouldReadSolutionFile() {
            var path = @"solutionPath\mysolution.sln";

            var loader = new Mock<IVisualStudioSolutionFileLoader>();
            loader.Setup(l => l.LoadVisualStudioSolution(path)).Returns(new VisualStudioSolutionFileDetails
            {VisualStudioProjects = new[] {
                new VisualStudioSolutionProject {Name = "Stuff", Path = @"Stuff\Stuff.csproj"},
                new VisualStudioSolutionProject {Name = "Other", Path = @"Other\Other.csproj"},
            }});

            var projectLoader = new Mock<IVisualStudioProjectFileLoader>();
            projectLoader
                .Setup(p => p.LoadProject(@"solutionPath\Stuff\Stuff.csproj", "Stuff", "configuration"))
                .Returns(new VisualStudioCSharpProjectFileDetails { OutputFile = @"bin\stuff.dll", Name = "Stuff" });
            projectLoader
                .Setup(p => p.LoadProject(@"solutionPath\Other\Other.csproj", "Other", "configuration"))
                .Returns(new VisualStudioCSharpProjectFileDetails { OutputFile = @"bin\other.dll", Name = "Other" });

            var reader = new VisualStudioSolutionFileReader(loader.Object, projectLoader.Object);

            var solution = reader.ReadSolution(path, "configuration");

            IEnumerable<VisualStudioProjectDetails> projects = solution.Projects;

            Assert.That(projects.Count(), Is.EqualTo(2));
            Assert.That(projects.ElementAt(0).OutputFile, Is.EqualTo(@"solutionPath\Stuff\bin\stuff.dll"));
            Assert.That(projects.ElementAt(0).Name, Is.EqualTo("Stuff"));
            Assert.That(projects.ElementAt(1).OutputFile, Is.EqualTo(@"solutionPath\Other\bin\other.dll"));
            Assert.That(projects.ElementAt(1).Name, Is.EqualTo("Other"));
        }
    }
}