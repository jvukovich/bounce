namespace Bounce.Framework {
    public interface IVisualStudioProjectFileLoader {
        VisualStudioCSharpProjectFileDetails LoadProject(string path, string projectName, string configuration);
    }
}