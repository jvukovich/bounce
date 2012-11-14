using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Bounce.Framework.VisualStudio {
    public class VisualStudioProjects : IEnumerable<VisualStudioProject> {
        private readonly IEnumerable<VisualStudioProject> Projects;

        public VisualStudioProjects(IEnumerable<VisualStudioProject> projects) {
            Projects = projects;
        }

        public VisualStudioProject this[string projectName] {
            get { return Projects.SingleOrDefault(p => p.Name == projectName); }
        }

        public IEnumerator<VisualStudioProject> GetEnumerator() {
            return Projects.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}