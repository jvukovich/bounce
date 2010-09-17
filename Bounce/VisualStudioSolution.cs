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

            new ShellCommandExecutor().ExecuteProcess("msbuild.exe", SolutionPath.Value, "msbiuld exited funny");

            LastBuilt = DateTime.UtcNow;
        }

        internal VisualStudioSolutionDetails SolutionDetails {
            get {
                var solutionPath = SolutionPath.Value;

                if (File.Exists(solutionPath)) {
                    return new VisualStudioSolutionFileReader().ReadSolution(solutionPath, Config);
                } else {
                    throw new DependencyBuildFailureException(this, String.Format("VisualStudio solution file `{0}' does not exist", solutionPath));
                }
            }
        }

        public void Clean() {
            Console.WriteLine("cleaning solution at: " + SolutionPath.Value);
        }

        public DateTime? LastBuilt { get; set; }

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

    [AttributeUsage(AttributeTargets.Method)]
    public class TargetsAttribute : Attribute {
    }
}