using System.IO;
using System.Linq;
using ICSharpCode.SharpZipLib.Zip;
using NUnit.Framework;

namespace Bounce.Framework.Tests {
    [TestFixture, Explicit("should be executed with MSBuild in PATH, say, from nunit from a Visual Studio command prompt.")]
    public class VisualStudioSolutionTest {
        private const string SolutionUnzipDirectory = "TestSolution";

        [Test]
        public void BuildsOutputFileOfFirstProject() {
            UnzipTestSolution();

            var solution = new VisualStudioSolution { SolutionPath = new PlainValue<string>(Path.Combine(SolutionUnzipDirectory, @"TestSolution\TestSolution.sln")) };

            solution.Build();

            Assert.That(File.Exists(solution.Projects["TestSolution"].OutputFile.Value));
        }

        [Test]
        public void CanAccessProjectsBeforeSolutionExists() {
            DeleteTestSolution();

            var solution = new VisualStudioSolution {SolutionPath = new PlainValue<string>(Path.Combine(SolutionUnzipDirectory, @"TestSolution\TestSolution.sln"))};

            var outputFiles = solution.Projects.Select(p => p.OutputFile);

            UnzipTestSolution();

            Assert.That(outputFiles.Select(o => o.Value).ToArray(), Is.EquivalentTo(new [] {@"TestSolution\TestSolution\TestSolution\bin\Debug\TestSolution.dll"}));
        }
        
        private void UnzipTestSolution() {
            DeleteTestSolution();
            new FastZip().ExtractZip("TestSolution.zip", SolutionUnzipDirectory, null);
        }

        private void DeleteTestSolution() {
            if (Directory.Exists(SolutionUnzipDirectory)) {
                Directory.Delete(SolutionUnzipDirectory, true);
            }
        }
    }
}