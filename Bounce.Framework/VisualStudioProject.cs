using System.Collections.Generic;

namespace Bounce.Framework {
    public class VisualStudioProject : Task {
        [Dependency]
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

        public Val<string> OutputDirectory {
            get {
                return this.WhenBuilt(() => Solution.GetProjectDetails(Name.Value).OutputDirectory);
            }
        }

        public Val<string> ProjectDirectory {
            get {
                return this.WhenBuilt(() => Solution.GetProjectDetails(Name.Value).ProjectDirectory);
            }
        }

        public Val<string> ProjectFile {
            get {
                return this.WhenBuilt(() => Solution.GetProjectDetails(Name.Value).ProjectFile);
            }
        }
    }
}