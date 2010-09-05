namespace Bounce.VisualStudio {
    public interface IVisualStudioProjectFileLoader {
        VisualStudioCSharpProjectFileDetails LoadProject(string path, string configuration);
    }
}