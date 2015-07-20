using Microsoft.Build.Utilities;

namespace Bounce.Framework.VisualStudio {
    public abstract class MsBuildFile : IMsBuildFile {
        public virtual IMsBuild MsBuild { get; set; }
        protected abstract string Path { get; }

        public void Build(string config = "Debug", string outputDir = null, string target = null, string verbosity = null, bool nologo = false, bool parallel = false,
			TargetDotNetFrameworkVersion dotNetFrameworkVersion = TargetDotNetFrameworkVersion.VersionLatest)
        {
            MsBuild.Build(Path, config, outputDir, target, verbosity, nologo, parallel, dotNetFrameworkVersion);
        }
    }
}