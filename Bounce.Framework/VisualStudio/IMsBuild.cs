namespace Bounce.Framework.VisualStudio
{
    public interface IMsBuild
    {
        void Build(string projSln, string config, string outputDir, string target, string verbosity, bool nologo, bool parallel, string msBuildToolsVersion, string customMsBuildToolsPath = null);
    }
}