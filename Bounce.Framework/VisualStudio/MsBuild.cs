using System;
using System.IO;
using System.Linq;
using Microsoft.Build.Utilities;

namespace Bounce.Framework.VisualStudio {
    class MsBuild : IMsBuild {
		private const string msbuildExe = "msbuild.exe";
        private readonly IShell Shell;
        public string MsBuildExe { get; set; }

        public MsBuild(IShell shell) {
            Shell = shell;
            MsBuildExe = Path.Combine(GetNetPath(TargetDotNetFrameworkVersion.VersionLatest), msbuildExe);
        }

        public void Build(string projSln, string config, string outputDir, string target, string verbosity, bool nologo, bool parallel, TargetDotNetFrameworkVersion dotNetFrameworkVersion)
        {
            var arguments = NormaliseArguments(
                "\"" + projSln + "\"",
                ConfigIfSpecified(config),
                OutputDirIfSpecified(outputDir),
                TargetIfSpecified(target),
				VerbosityIfSpecified(verbosity),
				NoLogoIfSpecified(nologo),
				ParallelIfSpecified(parallel));

			MsBuildExe = Path.Combine(GetNetPath(dotNetFrameworkVersion), msbuildExe);

            Shell.Exec(MsBuildExe, arguments);
        }

        private string NormaliseArguments(params string[] args)
        {
            return String.Join(" ", args.Where(a => a != null).ToArray());
        }

        protected string ConfigIfSpecified(string config)
        {
            return config != null ? "/p:Configuration=" + config : null;
        }

        protected string OutputDirIfSpecified(string outputDir)
        {
            return outputDir != null ? "/p:Outdir=" + EnsureTrailingSlashIsSet(outputDir) : null;
        }

        protected string TargetIfSpecified(string target)
        {
            return target != null ? "/t:" + target : null;
        }

		protected string VerbosityIfSpecified(string verbosity) {
			return verbosity != null ? "/verbosity:" + verbosity : null;
		}

		protected string NoLogoIfSpecified(bool nologo) {
			return nologo ? "/nologo" : null;
		}

		protected string ParallelIfSpecified(bool parallel) {
			return parallel ? "/m" : null;
		}

        private string EnsureTrailingSlashIsSet(string outputDir)
        {
            return outputDir.Last() == Path.DirectorySeparatorChar ? outputDir : outputDir + Path.DirectorySeparatorChar;
        }

		private static string GetNetPath(TargetDotNetFrameworkVersion dotNetFrameworkVersion) {
			return ToolLocationHelper.GetPathToDotNetFramework(dotNetFrameworkVersion);
		}
    }
}