using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Bounce.Framework.VisualStudio {
    public class VisualStudioSolution : MsBuildFile, IVisualStudioSolution {
        public string SolutionPath { get; set; }
        public IEnumerable<VisualStudioProject> VisualStudioProjects { get; set; }

        public VisualStudioSolution(string path) {
            SolutionPath = path;
        }

        public VisualStudioProjects Projects {
            get { return new VisualStudioProjects(VisualStudioProjects); }
        }

        protected override string Path {
            get { return SolutionPath; }
        }
    }
}