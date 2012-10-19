namespace Bounce.Framework.VisualStudio {
    public interface IVisualStudioProjectFileLoader {
        VisualStudioProject LoadProject(string path, string projectName, string configuration);
    }
}