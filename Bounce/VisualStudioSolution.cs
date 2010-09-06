using System;
using System.Collections.Generic;
using System.Linq;
using Bounce.VisualStudio;
using Microsoft.Build.BuildEngine;
using System.IO;

namespace Bounce {
    public class VisualStudioSolution : IIisWebSiteDirectory {
        public IValue<string> SolutionPath;
        public IValue<string> Configuration;
        public IValue<string> PrimaryProjectName;
        public IEnumerable<VisualStudioProjectDetails> Projects { get; private set; }
        public VisualStudioProjectDetails PrimaryProject { get; private set; }

        public IEnumerable<ITarget> Dependencies {
            get { return new[] {SolutionPath}; }
        }

        public void Build() {
            Console.WriteLine("building solution at: " + SolutionPath.Value);

            new ShellCommandExecutor().ExecuteProcess("msbuild.exe", SolutionPath.Value, "msbiuld exited funny");

            LastBuilt = DateTime.UtcNow;

            var reader = new VisualStudioSolutionFileReader();
            VisualStudioSolutionDetails solutionDetails = reader.ReadSolution(SolutionPath.Value, Config);
            Projects = solutionDetails.Projects;
            PrimaryProject = Projects.First(p => p.Name == PrimaryProjectName.Value);
        }

        public void Clean() {
            Console.WriteLine("cleaning solution at: " + SolutionPath.Value);
        }

        public DateTime? LastBuilt { get; set; }

        public string Path {
            get { return SolutionPath.Value; }
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
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class TargetsAttribute : Attribute {
    }
}