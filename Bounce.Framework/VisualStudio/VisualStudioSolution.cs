using System.Collections.Generic;

namespace Bounce.Framework.VisualStudio {
    public class VisualStudioSolution : MsBuildFile, IVisualStudioSolution {
        private readonly string Configuration;
        public string SolutionPath { get; private set; }
        private readonly IEnumerable<VisualStudioSolutionProjectReference> ProjectReferences;

        public VisualStudioSolution(string path, string configuration, IEnumerable<VisualStudioSolutionProjectReference> projectReferences) {
            Configuration = configuration;
            ProjectReferences = projectReferences;
            SolutionPath = path;
        }

        public VisualStudioProjects Projects {
            get { return new VisualStudioProjects(this, ProjectReferences, Configuration); }
        }

        protected override string Path {
            get { return SolutionPath; }
        }
    }
}