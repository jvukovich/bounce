using System;
using System.Collections.Generic;
using System.Linq;
using Bounce.VisualStudio;
using System.IO;

namespace Bounce.Framework {
    public class VS {
        public static VisualStudioSolution Solution(IValue<string> path) {
            return new VisualStudioSolution {SolutionPath = path};
        }
    }

    public class VisualStudioSolution : ITarget {
        public IValue<string> SolutionPath;
        public IValue<string> Configuration;
        private readonly ShellCommandExecutor ShellCommandExecutor;

        public VisualStudioSolution() {
            ShellCommandExecutor = new ShellCommandExecutor();
        }

        public VisualStudioSolutionProjects Projects {
            get {
                return new VisualStudioSolutionProjects(this);
            }
        }

        public IEnumerable<ITarget> Dependencies {
            get { return new[] {SolutionPath}; }
        }

        public void BeforeBuild() {
        }

        public void Build() {
            Console.WriteLine("building solution at: " + SolutionPath.Value);

            ShellCommandExecutor.ExecuteAndExpectSuccess("msbuild.exe", String.Format(@"""{0}""", SolutionPath.Value));
        }

        internal VisualStudioSolutionDetails SolutionDetails {
            get {
                var details = TryGetSolutionDetails();

                if (details != null) {
                    return details;
                } else {
                    throw new DependencyBuildFailureException(this,
                                                              String.Format("VisualStudio solution file `{0}' does not exist",
                                                                            SolutionPath.Value));
                }
            }
        }

        internal VisualStudioSolutionDetails TryGetSolutionDetails() {
            var solutionPath = SolutionPath.Value;

            if (File.Exists(solutionPath)) {
                return new VisualStudioSolutionFileReader().ReadSolution(solutionPath, Config);
            } else {
                return null;
            }
        }

        public void Clean() {
            Console.WriteLine("cleaning solution at: " + SolutionPath.Value);
            ShellCommandExecutor.ExecuteAndExpectSuccess("msbuild.exe", String.Format(@"/target:Clean ""{0}""", SolutionPath.Value));
        }

        private string Config {
            get {
                if (Configuration == null) {
                    return "";
                } else {
                    return Configuration.Value;
                }
            }
        }

        internal VisualStudioProjectDetails GetProjectDetails(string name) {
            return SolutionDetails.Projects.First(p => p.Name == name);
        }
    }
}