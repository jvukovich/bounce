using System.Collections.Generic;

namespace Bounce.Framework {
    public class VisualStudioProject : Task {
        [Dependency]
        private readonly VisualStudioSolution Solution;

        public VisualStudioProject(VisualStudioSolution solution, Future<string> name) {
            Solution = solution;
            Name = name;
        }

        public Future<string> Name { get; private set; }

        public Future<string> OutputFile {
            get {
                return this.WhenBuilt(() => Solution.GetProjectDetails(Name.Value).OutputFile);
            }
        }

        public Future<string> OutputDirectory {
            get {
                return this.WhenBuilt(() => Solution.GetProjectDetails(Name.Value).OutputDirectory);
            }
        }

        public Future<string> ProjectDirectory {
            get {
                return this.WhenBuilt(() => Solution.GetProjectDetails(Name.Value).ProjectDirectory);
            }
        }

        public Future<string> ProjectFile {
            get {
                return this.WhenBuilt(() => Solution.GetProjectDetails(Name.Value).ProjectFile);
            }
        }
    }
}