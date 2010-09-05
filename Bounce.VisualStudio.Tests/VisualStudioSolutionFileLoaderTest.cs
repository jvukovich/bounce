using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;

namespace Bounce.VisualStudio.Tests {
    [TestFixture]
    public class VisualStudioSolutionFileLoaderTest {
        [Test]
        public void ShouldReadProjectsFromSolutionFile()
        {
            string path = @"TestFiles\Bounce.sln";

            var reader = new VisualStudioSolutionFileLoader();
            VisualStudioSolutionFileDetails details = reader.LoadVisualStudioSolution(path);

            VisualStudioSolutionProject bounce = details.VisualStudioProjects.ElementAt(0);
            Assert.That(bounce.Name, Is.EqualTo("Bounce"));
            Assert.That(bounce.Path, Is.EqualTo(@"Bounce\Bounce.csproj"));

            VisualStudioSolutionProject bouncerConsole = details.VisualStudioProjects.ElementAt(1);
            Assert.That(bouncerConsole.Name, Is.EqualTo("Bouncer.Console"));
            Assert.That(bouncerConsole.Path, Is.EqualTo(@"Bouncer.Console\Bouncer.Console.csproj"));
        }
    }

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

    [TestFixture]
    public class ProjectFilePropertyExpressionParserTest {
        [Test]
        public void ShouldParseString() {
            var parser = new ProjectFilePropertyExpressionParser(null);
            AssertParseStringResult(parser, "'one'", "one");
        }

        [Test]
        public void ShouldParseVariableInString() {
            var props = new PropertyValues();
            props["var"] = "value";

            var parser = new ProjectFilePropertyExpressionParser(props);
            AssertParseStringResult(parser, "'$(var)'", "value");
        }

        [Test]
        public void ShouldParseCondition() {
            var props = new PropertyValues();
            props["var"] = "value";

            var parser = new ProjectFilePropertyExpressionParser(props);
            AssertParseConditionResult(parser, " '$(var)' == 'value' ", true);
            AssertParseConditionResult(parser, " '$(var)' == 'valu' ", false);
            AssertParseConditionResult(parser, " '$(var) ' == 'value' ", false);
        }

        private void AssertParseStringResult(ProjectFilePropertyExpressionParser parser, string source, string expected) {
            var result = parser.Parse<string>(source, parser.ParseString);
            Assert.That(result, Is.EqualTo(expected));
        }

        private void AssertParseConditionResult(ProjectFilePropertyExpressionParser parser, string source, bool expectedResult) {
            var result = parser.Parse<bool>(source, parser.ParseEqualityExpression);
            Assert.That(result, Is.EqualTo(expectedResult));
        }
    }

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
