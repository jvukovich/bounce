namespace Bounce.Framework.VisualStudio {
    public interface IVisualStudioProject : IMsBuildFile {
        string OutputFile { get; }
        string Name { get; }
        string OutputDirectory { get; }
        string ProjectFile { get; }
        string ProjectDirectory { get; }
    }
}