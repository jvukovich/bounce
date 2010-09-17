using System.Linq;
using Bounce.Framework;

namespace TestBounceAssembly {
    public class BuildTargets {
        [Targets]
        public static object Targets (IParameters parameters) {
            var solution = VS.Solution(parameters.Default("sln", @"C:\Users\Public\Documents\Development\BigSolution\BigSolution.sln"));
            var project = solution.Projects[parameters.Default("proj", "BigSolution")];
            var serviceName = parameters.Default("svc", "BigWindowsService");
            var service = solution.Projects[serviceName];

            return new {
                WebSite = new Iis7WebSite() {Path = project.Directory, Name = "BigWebSite".V(), Port = 5001.V()},
                Tests = new NUnitTestResults() {DllPaths = solution.Projects.Select(p => p.OutputFile)},
                Service = new WindowsService {BinaryPath = service.OutputFile, Name = serviceName, UserName = @"sonomorph\tim".V(), Password = "fAct04Fv".V()},
            };
        }
    }
}
