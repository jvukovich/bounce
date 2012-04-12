using System.Linq;

namespace Bounce.Framework.VisualStudio {
    public class VisualStudioProjects {
        private readonly VisualStudioSolutionDetails Solution;

        public VisualStudioProjects(VisualStudioSolutionDetails solution) {
            Solution = solution;
        }

        public VisualStudioProjectFileDetails this[string projectName] {
            get { return Solution.Projects.SingleOrDefault(p => p.Name == projectName); }
        }
    }
}