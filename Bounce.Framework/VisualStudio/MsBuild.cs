using System;
using System.IO;
using System.Linq;

namespace Bounce.Framework.VisualStudio {
    class MsBuild : IMsBuild {
        private readonly IShell Shell;
        public string MsBuildExe { get; set; }

        public MsBuild(IShell shell) {
            Shell = shell;
            MsBuildExe = Environment.ExpandEnvironmentVariables(@"%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\msbuild.exe");
        }

        public void Build(string projSln, string config, string outputDir, string target)
        {
            var arguments = NormaliseArguments(
                "\"" + projSln + "\"",
                ConfigIfSpecified(config),
                OutputDirIfSpecified(outputDir),
                TargetIfSpecified(target)
                );

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

        private string EnsureTrailingSlashIsSet(string outputDir)
        {
            return outputDir.Last() == Path.DirectorySeparatorChar ? outputDir : outputDir + Path.DirectorySeparatorChar;
        }
    }
}