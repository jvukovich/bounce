namespace Bounce.Framework.Obsolete {
    public class VisualStudioProject : Task {
        [Dependency]
        private readonly VisualStudioSolution Solution;

        public VisualStudioProject(VisualStudioSolution solution, Task<string> name) {
            Solution = solution;
            Name = name;
        }

        public Task<string> Name { get; private set; }

        public Task<string> OutputFile {
            get {
                return this.WhenBuilt(() => Solution.GetProjectDetails(Name.Value).OutputFile);
            }
        }

        public Task<string> OutputDirectory {
            get {
                return this.WhenBuilt(() => Solution.GetProjectDetails(Name.Value).OutputDirectory);
            }
        }

        public Task<string> ProjectDirectory {
            get {
                return this.WhenBuilt(() => Solution.GetProjectDetails(Name.Value).ProjectDirectory);
            }
        }

        public Task<string> ProjectFile {
            get {
                return this.WhenBuilt(() => Solution.GetProjectDetails(Name.Value).ProjectFile);
            }
        }
    }
}