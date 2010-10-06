using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace Bounce.Framework {
    public class VisualStudioSolution : Task {
        [Dependency]
        public Val<string> SolutionPath;
        [Dependency]
        public Val<string> Configuration;
        [Dependency]
        public Val<string> MsBuildExe;

        private readonly IShellCommandExecutor ShellCommandExecutor;

        public VisualStudioSolution() {
            ShellCommandExecutor = new ShellCommandExecutor();
            MsBuildExe = @"C:\Windows\Microsoft.NET\Framework\v3.5\msbuild.exe";
        }

        public VisualStudioSolutionProjects Projects {
            get {
                return new VisualStudioSolutionProjects(this);
            }
        }

        public override void Build(IBounce bounce) {
            bounce.Log.Info("building solution at: " + SolutionPath.Value);

            ShellCommandExecutor.ExecuteAndExpectSuccess(MsBuildExe.Value, String.Format(@"""{0}""", SolutionPath.Value));
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
            if (SolutionExists) {
                return new VisualStudioSolutionFileReader().ReadSolution(SolutionPath.Value, Config);
            } else {
                return null;
            }
        }

        private bool SolutionExists {
            get {
                return File.Exists(SolutionPath.Value);
            }
        }

        public override void Clean(IBounce bounce) {
            if (SolutionExists) {
                bounce.Log.Info("cleaning solution at: " + SolutionPath.Value);
                ShellCommandExecutor.ExecuteAndExpectSuccess("msbuild.exe", String.Format(@"/target:Clean ""{0}""", SolutionPath.Value));
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
}