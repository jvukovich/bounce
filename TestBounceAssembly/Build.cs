using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bounce;

namespace TestBounceAssembly {
    public class Build {
        [Targets]
        public static object Targets (IParameters parameters) {
            var solution = new VisualStudioSolution() { SolutionPath = parameters.Default("sln", @"..\..\..\Bounce.sln"), PrimaryProjectName = parameters.Default("proj", "Bouncer.Console") };

            return new {
                Default = new IisWebSite() { WebSiteDirectory = solution },
                Tests = new NUnitTestResults() { DllPaths = solution.Property(s => s.Projects.Select(p => p.OutputFile)) },
            };
        }
    }
}
