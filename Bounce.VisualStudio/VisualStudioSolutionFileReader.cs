using System.Collections.Generic;
using System.IO;

namespace Bounce.VisualStudio {
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

            var projects = new List<VisualStudioProjectDetails>();

            foreach (var project in solutionDetails.VisualStudioProjects) {
                string projectPath = Path.Combine(Path.GetDirectoryName(solutionPath), project.Path);
                VisualStudioCSharpProjectFileDetails projectDetails = ProjectLoader.LoadProject(projectPath, project.Name, configuration);
                string outputPath = Path.Combine(Path.GetDirectoryName(projectPath), projectDetails.OutputFile);
                projects.Add(new VisualStudioProjectDetails { Name = projectDetails.Name, OutputFile = outputPath });
            }

            return new VisualStudioSolutionDetails {Projects = projects};
        }
    }
}