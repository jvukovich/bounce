namespace Bounce.Framework.Obsolete {
    public interface IVisualStudioProjectFileLoader {
        VisualStudioProjectFileDetails LoadProject(string path, string projectName, string configuration);
    }
}