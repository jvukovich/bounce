using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Bounce.VisualStudio;
using Microsoft.Build.BuildEngine;
using System.IO;

namespace Bounce.Framework {
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

        public void Build() {
            Console.WriteLine("building solution at: " + SolutionPath.Value);

            ShellCommandExecutor.ExecuteProcess("msbuild.exe", String.Format(@"""{0}""", SolutionPath.Value));
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
            ShellCommandExecutor.ExecuteProcess("msbuild.exe", String.Format(@"/target:Clean ""{0}""", SolutionPath.Value));
        }

        public DateTime? LastBuilt {
            get {
                var details = TryGetSolutionDetails();

                if (details != null) {
                    IEnumerable<DateTime> outputFileModTimes = SolutionDetails.Projects
                        .Select(p => p.OutputFile)
                        .Where(File.Exists)
                        .Select(filename => File.GetLastWriteTimeUtc(filename));

                    if (outputFileModTimes.Count() > 0) {
                        return outputFileModTimes.Max();
                    } else {
                        return null;
                    }
                } else {
                    return null;
                }
            }
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

    public class VisualStudioSolutionProjects : IEnumerable<VisualStudioProject> {
        private readonly VisualStudioSolution solution;

        public VisualStudioSolutionProjects(VisualStudioSolution solution) {
            this.solution = solution;
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        public IEnumerator<VisualStudioProject> GetEnumerator() {
            return solution.SolutionDetails.Projects.Select(p => new VisualStudioProject(solution, new PlainValue<string>(p.Name))).GetEnumerator();
        }

        public VisualStudioProject this[IValue<String> name] {
            get {
                return new VisualStudioProject(solution, name);
            }
        }
    }

    internal class DependencyBuildFailureException : TargetException {
        public DependencyBuildFailureException(ITarget target, string message) : base(target, message) {
        }
    }

    internal class TargetException : BounceException {
        public ITarget Target { get; private set; }

        public TargetException(ITarget target, string message) : base(message) {
            Target = target;
        }
    }

    internal class BounceException : Exception {
        public BounceException(string message) : base(message) {
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class TargetsAttribute : Attribute {
    }
}