using System;
using System.Collections.Generic;

namespace Bounce.Framework {
    public class VisualStudioProject : IIisWebSiteDirectory {
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

        public IEnumerable<ITarget> Dependencies {
            get { return new[] {Solution}; }
        }

        public DateTime? LastBuilt {
            get { return Solution.LastBuilt; }
        }

        public void Build() {
        }

        public void Clean() {
        }

        public IValue<string> Path {
            get { return this.Future(() => System.IO.Path.GetDirectoryName(OutputFile.Value)); }
        }
    }
}