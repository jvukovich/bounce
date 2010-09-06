namespace Bounce.VisualStudio {
    public interface IVisualStudioSolutionFileLoader {
        VisualStudioSolutionFileDetails LoadVisualStudioSolution(string path);
    }
}