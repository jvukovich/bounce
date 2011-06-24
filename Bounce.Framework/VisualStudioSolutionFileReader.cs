using System.Collections.Generic;
using System.IO;

namespace Bounce.Framework {
    public class VisualStudioSolutionFileReader {
        private readonly IVisualStudioSolutionFileLoader SolutionLoader;
        private readonly IVisualStudioProjectFileLoader ProjectLoader;

        public VisualStudioSolutionFileReader()
            : this(new VisualStudioSolutionFileLoader(), new VisualStudioCSharpProjectFileLoader()) {}

        public VisualStudioSolutionFileReader(IVisualStudioSolutionFileLoader solutionLoader, IVisualStudioProjectFileLoader projectLoader) {
            SolutionLoader = solutionLoader;
            ProjectLoader = projectLoader;
        }

        public VisualStudioSolutionDetails ReadSolution(string solutionPath, string configuration) {
            VisualStudioSolutionFileDetails solutionDetails = SolutionLoader.LoadVisualStudioSolution(solutionPath);

            var projects = new List<VisualStudioProjectFileDetails>();

            foreach (var project in solutionDetails.VisualStudioProjects) {
                if (Path.GetExtension(project.Path) == ".csproj") {
                    string projectFileName = Path.Combine(Path.GetDirectoryName(solutionPath), project.Path);
                    VisualStudioProjectFileDetails projectDetails = ProjectLoader.LoadProject(projectFileName,
                                                                                                    project.Name,
                                                                                                    configuration);

                    projects.Add(projectDetails);
                }
            }

            return new VisualStudioSolutionDetails {Projects = projects};
        }
    }
}