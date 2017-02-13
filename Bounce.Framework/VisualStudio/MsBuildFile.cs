namespace Bounce.Framework.VisualStudio {
    public abstract class MsBuildFile : IMsBuildFile {
        public virtual IMsBuild MsBuild { get; set; }
        protected abstract string Path { get; }

        public void Build(string config = "Debug", string outputDir = null, string target = null, string verbosity = null, bool nologo = false, bool parallel = false,
			string msBuildToolsVersion = MsBuildToolsVersion.VersionLatest, string customMsBuildToolsPath = null)
        {
            MsBuild.Build(Path, config, outputDir, target, verbosity, nologo, parallel, msBuildToolsVersion, customMsBuildToolsPath);
        }
    }
}