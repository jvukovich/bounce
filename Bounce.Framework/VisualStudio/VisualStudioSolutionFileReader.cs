using System.Collections.Generic;
using System.IO;

namespace Bounce.Framework.VisualStudio {
    public class VisualStudioSolutionFileReader {
        private readonly IVisualStudioSolutionFileLoader SolutionLoader;
        private readonly IVisualStudioProjectFileLoader ProjectLoader;

        public VisualStudioSolutionFileReader()
            : this(new VisualStudioSolutionFileLoader(), new VisualStudioCSharpProjectFileLoader()) {}

        public VisualStudioSolutionFileReader(IVisualStudioSolutionFileLoader solutionLoader, IVisualStudioProjectFileLoader projectLoader) {
            SolutionLoader = solutionLoader;
            ProjectLoader = projectLoader;
        }

        public VisualStudioSolution ReadSolution(string solutionPath, string configuration) {
            VisualStudioSolutionFileDetails solutionDetails = SolutionLoader.LoadVisualStudioSolution(solutionPath);

            var projects = new List<VisualStudioProject>();
            var sln = new VisualStudioSolution(solutionPath) { VisualStudioProjects = projects };

            foreach (var project in solutionDetails.VisualStudioProjects) {
                if (Path.GetExtension(project.Path) == ".csproj") {
                    string projectFileName = Path.Combine(Path.GetDirectoryName(solutionPath), project.Path);
                    VisualStudioProject projectDetails = ProjectLoader.LoadProject(projectFileName,
                                                                                   project.Name,
                                                                                   configuration);

                    projectDetails.Solution = sln;
                    projects.Add(projectDetails);
                }
            }

            return sln;
        }
    }
}