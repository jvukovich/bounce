using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Bounce.Framework.VisualStudio {
    public class VisualStudioProjects : IEnumerable<VisualStudioProjectFileDetails> {
        private readonly VisualStudioSolutionDetails Solution;

        public VisualStudioProjects(VisualStudioSolutionDetails solution) {
            Solution = solution;
        }

        public VisualStudioProjectFileDetails this[string projectName] {
            get { return Solution.Projects.SingleOrDefault(p => p.Name == projectName); }
        }

        public IEnumerator<VisualStudioProjectFileDetails> GetEnumerator() {
            return Solution.Projects.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}