using System;
using System.IO;
using System.Linq;

namespace LegacyBounce.Framework {
    public class VisualStudioSolution : Task {
        [Dependency]
        public Task<string> SolutionPath;
        [Dependency]
        public Task<string> Configuration;
        [Dependency]
        public Task<string> OutputDir;
        [Dependency]
        public Task<string> Target;
        [Dependency]
        public Task<string> MsBuildExe;

        public VisualStudioSolution() {
            MsBuildExe = Environment.ExpandEnvironmentVariables(@"%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\msbuild.exe");
        }

        [Obsolete("Use Projects")]
        public VisualStudioSolutionProjectsObsolete ProjectsObsolete {
            get {
                return new VisualStudioSolutionProjectsObsolete(this);
            }
        }

        public VisualStudioSolutionProjects Projects {
            get {
                return new VisualStudioSolutionProjects(this);
            }
        }

        public string SolutionDirectory {
            get {
                if (SolutionPath == null || SolutionPath.Value == null) {
                    return null;
                }

                return Path.GetDirectoryName(SolutionPath.Value);
            }
        }

        public override void Build(IBounce bounce) {
            bounce.Log.Info("building solution at: " + SolutionPath.Value);

            var arguments = Arguments(
                "\"" + SolutionPath.Value + "\"",
                ConfigIfSpecified,
                OutputDirIfSpecified,
                TargetIfSpecified
            );

            bounce.ShellCommand.ExecuteAndExpectSuccess(MsBuildExe.Value, arguments);
        }

        private static string Arguments(params string[] args) {
            return String.Join(" ", args.Where(a => a != null).ToArray());
        }

        protected string ConfigIfSpecified {
            get {
                if (Configuration == null || Configuration.Value == null) {
                    return null;
                }
                else {
                    return "/p:Configuration=" + Configuration.Value;
                }
            }
        }

        protected string OutputDirIfSpecified {
            get {
                if (OutputDir == null || OutputDir.Value == null) {
                    return null;
                }

                return "/p:Outdir=" + EnsureTrailingSlashIsSet(OutputDir.Value);
            }
        }

        protected string TargetIfSpecified {
            get {
                if (Target == null || Target.Value == null) {
                    return null;
                }

                return "/t:" + Target.Value;
            }
        }

        private string EnsureTrailingSlashIsSet(string outputDir) {
            return outputDir.Last<char>() == Path.DirectorySeparatorChar ? outputDir : outputDir + Path.DirectorySeparatorChar;
        }

        internal VisualStudioSolutionDetails SolutionDetails {
            get {
                var details = TryGetSolutionDetails();

                if (details != null) {
                    return details;
                }
                else {
                    throw new DependencyBuildFailureException(this,
                                                              String.Format("VisualStudio solution file `{0}' does not exist",
                                                                            SolutionPath.Value));
                }
            }
        }

        internal VisualStudioSolutionDetails TryGetSolutionDetails() {
            if (SolutionExists) {
                return new VisualStudioSolutionFileReader().ReadSolution(SolutionPath.Value, Config);
            }
            else {
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
                bounce.ShellCommand.ExecuteAndExpectSuccess(MsBuildExe.Value, String.Format(@"/target:Clean ""{0}""", SolutionPath.Value));
            }
        }

        private string Config {
            get {
                if (Configuration == null) {
                    return "";
                }
                else {
                    return Configuration.Value;
                }
            }
        }

        internal VisualStudioProjectFileDetails GetProjectDetails(string name) {
            return SolutionDetails.Projects.First(p => p.Name == name);
        }
    }
}