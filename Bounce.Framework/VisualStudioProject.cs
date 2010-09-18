using System.Collections.Generic;

namespace Bounce.Framework {
    public class VisualStudioProject : ITask {
        private readonly VisualStudioSolution Solution;

        public VisualStudioProject(VisualStudioSolution solution, Val<string> name) {
            Solution = solution;
            Name = name;
        }

        public Val<string> Name { get; private set; }
        public Val<string> OutputFile {
            get {
                return this.WhenBuilt(() => Solution.GetProjectDetails(Name.Value).OutputFile);
            }
        }
        public Val<string> Directory {
            get {
                return this.WhenBuilt(() => Solution.GetProjectDetails(Name.Value).Directory);
            }
        }

        public IEnumerable<ITask> Dependencies {
            get { return new[] {Solution}; }
        }

        public void BeforeBuild() {
        }

        public void Build() {
        }

        public void Clean() {
        }
    }
}