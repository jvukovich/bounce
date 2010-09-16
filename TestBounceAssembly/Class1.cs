using System.Linq;
using Bounce;
using Bounce.Framework;

namespace TestBounceAssembly {
    public class Class1 {
        [Targets]
        public static object Targets (IParameters parameters) {
            var solution = new VisualStudioSolution() { SolutionPath = parameters.Default("sln", @"..\..\..\Bounce.sln") };
            var bounce = solution.Projects[parameters.Default("proj", "Bouncer.Console")];

            return new {
                Default = new IisWebSite() { WebSiteDirectory = bounce },
                Tests = new NUnitTestResults() { DllPaths = solution.Projects.Select(p => p.OutputFile) },
            };
        }
    }
}
