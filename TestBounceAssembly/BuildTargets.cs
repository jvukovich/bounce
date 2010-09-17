using System.Linq;
using Bounce.Framework;

namespace TestBounceAssembly {
    public class BuildTargets {
        [Targets]
        public static object Targets (IParameters parameters) {
            var solution = new VisualStudioSolution() { SolutionPath = parameters.Default("sln", @"C:\Users\Public\Documents\Development\BigSolution\BigSolution.sln") };
            var project = solution.Projects[parameters.Default("proj", "BigSolution")];

            return new {
                Default = new IisWebSite() { WebSiteDirectory = project },
                Tests = new NUnitTestResults() { DllPaths = solution.Projects.Select(p => p.OutputFile) },
            };
        }
    }
}
