namespace Bounce.Framework {
    public interface IVisualStudioSolutionFileLoader {
        VisualStudioSolutionFileDetails LoadVisualStudioSolution(string path);
    }
}