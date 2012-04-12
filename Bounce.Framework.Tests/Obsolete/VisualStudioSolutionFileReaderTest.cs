using System.Collections.Generic;
using Bounce.Framework.Obsolete;
using Moq;
using NUnit.Framework;

namespace Bounce.Framework.Tests.Obsolete {
    [TestFixture]
    public class VisualStudioSolutionFileReaderTest {
        [Test]
        public void ShouldReadSolutionFile() {
            var path = @"solutionPath\mysolution.sln";

            var loader = new Mock<IVisualStudioSolutionFileLoader>();
            loader.Setup(l => l.LoadVisualStudioSolution(path)).Returns(new VisualStudioSolutionFileDetails
            {VisualStudioProjects = new[] {
                new VisualStudioSolutionProjectReference {Name = "Stuff", Path = @"Stuff\Stuff.csproj"},
                new VisualStudioSolutionProjectReference {Name = "Other", Path = @"Other\Other.csproj"},
            }});

            var projectLoader = new Mock<IVisualStudioProjectFileLoader>();
            var stuff = new VisualStudioProjectFileDetails { OutputFile = @"solutionPath\Stuff\bin\stuff.dll", Name = "Stuff", ProjectDirectory = @"solutionPath\Stuff" };
            var other = new VisualStudioProjectFileDetails { OutputFile = @"solutionPath\Other\bin\other.dll", Name = "Other", ProjectDirectory = @"solutionPath\Other"};

            projectLoader
                .Setup(p => p.LoadProject(@"solutionPath\Stuff\Stuff.csproj", "Stuff", "configuration"))
                .Returns(stuff);
            projectLoader
                .Setup(p => p.LoadProject(@"solutionPath\Other\Other.csproj", "Other", "configuration"))
                .Returns(other);

            var reader = new VisualStudioSolutionFileReader(loader.Object, projectLoader.Object);

            var solution = reader.ReadSolution(path, "configuration");

            IEnumerable<VisualStudioProjectFileDetails> projects = solution.Projects;

            Assert.That(projects, Is.EquivalentTo(new[] {stuff, other}));
        }
    }
}