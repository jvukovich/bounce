using System;
using System.Linq;

namespace Bounce.Framework.VisualStudio {
    public class VisualStudioSolution : IVisualStudioSolution {
        public string MsBuildExe { get; set; }
        public string SolutionPath { get; set; }
        public string Configuration { get; set; }
        public string OutputDir { get; set; }
        public string Target { get; set; }

        private VisualStudioSolutionFileReader SolutionReader;

        public VisualStudioSolution(string path) {
            SolutionPath = path;
            MsBuildExe = Environment.ExpandEnvironmentVariables(@"%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\msbuild.exe");
            SolutionReader = new VisualStudioSolutionFileReader();
            Configuration = "Debug";
        }

        public void Build() {
            var arguments = Arguments(
                "\"" + SolutionPath + "\"",
                ConfigIfSpecified,
                OutputDirIfSpecified,
                TargetIfSpecified
            );

            Bounce.Shell.ExecuteAndExpectSuccess(MsBuildExe, arguments);
        }

        public VisualStudioProjects Projects {
            get {
                return new VisualStudioProjects(SolutionReader.ReadSolution(SolutionPath, Configuration));
            }
        }

        private static string Arguments(params string[] args)
        {
            return String.Join(" ", args.Where(a => a != null).ToArray());
        }

        protected string ConfigIfSpecified
        {
            get
            {
                if (Configuration == null)
                {
                    return null;
                }
                else
                {
                    return "/p:Configuration=" + Configuration;
                }
            }
        }

        protected string OutputDirIfSpecified
        {
            get
            {
                if (OutputDir == null)
                {
                    return null;
                }

                return "/p:Outdir=" + EnsureTrailingSlashIsSet(OutputDir);
            }
        }

        protected string TargetIfSpecified
        {
            get
            {
                if (Target == null)
                {
                    return null;
                }

                return "/t:" + Target;
            }
        }

        private string EnsureTrailingSlashIsSet(string outputDir)
        {
            return outputDir.Last<char>() == System.IO.Path.DirectorySeparatorChar ? outputDir : outputDir + System.IO.Path.DirectorySeparatorChar;
        }
    }
}