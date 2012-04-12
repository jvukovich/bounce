using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LegacyBounce.Framework {
    public class VisualStudioSolutionProjectsObsolete : IEnumerable<VisualStudioProject> {
        private readonly VisualStudioSolution solution;

        public VisualStudioSolutionProjectsObsolete(VisualStudioSolution solution) {
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

    public class VisualStudioSolutionProjects : TaskWithValue<IEnumerable<VisualStudioProjectFileDetails>>
    {
        [Dependency]
        private readonly Task<IEnumerable<VisualStudioProjectFileDetails>> ProjectDetails;
        [Dependency]
        private readonly VisualStudioSolution Solution;

        public VisualStudioSolutionProjects(VisualStudioSolution solution)
        {
            ProjectDetails = solution.WhenBuilt(() => solution.SolutionDetails.Projects);
            Solution = solution;
        }

        protected override IEnumerable<VisualStudioProjectFileDetails> GetValue()
        {
            return ProjectDetails.Value;
        }

        public VisualStudioProject this [Task<string> name]
        {
            get { return new VisualStudioProject(Solution, name); }
        }
    }
}