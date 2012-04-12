namespace LegacyBounce.Framework {
    public interface IVisualStudioProjectFileLoader {
        VisualStudioProjectFileDetails LoadProject(string path, string projectName, string configuration);
    }
}