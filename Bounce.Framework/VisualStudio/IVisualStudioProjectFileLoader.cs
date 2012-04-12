namespace Bounce.Framework.VisualStudio {
    public interface IVisualStudioProjectFileLoader {
        VisualStudioProjectFileDetails LoadProject(string path, string projectName, string configuration);
    }
}