using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Bounce.Framework {
    public class VisualStudioSolutionProjects : IEnumerable<VisualStudioProject> {
        private readonly VisualStudioSolution solution;

        public VisualStudioSolutionProjects(VisualStudioSolution solution) {
            this.solution = solution;
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        public IEnumerator<VisualStudioProject> GetEnumerator() {
            return solution.SolutionDetails.Projects.Select(p => new VisualStudioProject(solution, new ImmediateValue<string>(p.Name))).GetEnumerator();
        }

        public VisualStudioProject this[Task<string> name] {
            get {
                return new VisualStudioProject(solution, name);
            }
        }
    }
}