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
                new VisualStudioSolutionProject {Name = "Stuff", Path = "StuffPath.csproj"},
                new VisualStudioSolutionProject {Name = "Other", Path = "OtherPath.csproj"},
            }});

            var projectLoader = new Mock<IVisualStudioProjectFileLoader>();
            projectLoader
                .Setup(p => p.LoadProject(@"solutionPath\StuffPath.csproj", "configuration"))
                .Returns(new VisualStudioCSharpProjectFileDetails { OutputFile = "stuff.dll" });
            projectLoader
                .Setup(p => p.LoadProject(@"solutionPath\OtherPath.csproj", "configuration"))
                .Returns(new VisualStudioCSharpProjectFileDetails { OutputFile = "other.dll" });

            var reader = new VisualStudioSolutionFileReader(loader.Object, projectLoader.Object);

            var solution = reader.ReadSolution(path, "configuration");

            IEnumerable<VisualStudioCSharpProjectFileDetails> projects = solution.Projects;

            Assert.That(projects.Count(), Is.EqualTo(2));
            Assert.That(projects.ElementAt(0).OutputFile, Is.EqualTo("stuff.dll"));
            Assert.That(projects.ElementAt(1).OutputFile, Is.EqualTo("other.dll"));
        }
    }
}