using System.Collections.Generic;

namespace Bounce.Framework {
    public class VisualStudioProject : ITarget {
        private readonly VisualStudioSolution Solution;

        public VisualStudioProject(VisualStudioSolution solution, IValue<string> name) {
            Solution = solution;
            Name = name;
        }

        public IValue<string> Name { get; private set; }
        public IValue<string> OutputFile {
            get {
                return this.Future(() => Solution.GetProjectDetails(Name.Value).OutputFile);
            }
        }
        public IValue<string> Directory {
            get {
                return this.Future(() => Solution.GetProjectDetails(Name.Value).Directory);
            }
        }

        public IEnumerable<ITarget> Dependencies {
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