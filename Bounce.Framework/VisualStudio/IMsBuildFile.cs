namespace Bounce.Framework.VisualStudio {
    public interface IMsBuildFile {
        void Build(string config = "Debug", string outputDir = null, string target = null);
    }
}