namespace Bounce.Framework.VisualStudio
{
    public interface IMsBuildFile
    {
        void Build(string config = "Debug", string outputDir = null, string target = null, string verbosity = null, bool nologo = false, bool parallel = false,
            string msBuildToolsVersion = MsBuildToolsVersion.VersionLatest, string customMsBuildToolsPath = null);
    }
}