namespace LegacyBounce.Framework {
    public interface IVisualStudioSolutionFileLoader {
        VisualStudioSolutionFileDetails LoadVisualStudioSolution(string path);
    }
}