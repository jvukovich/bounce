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
                string projectFileName = Path.Combine(Path.GetDirectoryName(solutionPath), project.Path);
                VisualStudioCSharpProjectFileDetails projectDetails = ProjectLoader.LoadProject(projectFileName, project.Name, configuration);

                string projectDirectory = Path.GetDirectoryName(projectFileName);
                string outputPath = Path.Combine(projectDirectory, projectDetails.OutputFile);
                projects.Add(new VisualStudioProjectDetails { Name = projectDetails.Name, OutputFile = outputPath, Directory = projectDirectory});
            }

            return new VisualStudioSolutionDetails {Projects = projects};
        }
    }
}