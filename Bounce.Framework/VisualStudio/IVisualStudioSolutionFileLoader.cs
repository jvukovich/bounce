namespace Bounce.Framework.VisualStudio {
    public interface IVisualStudioSolutionFileLoader {
        VisualStudioSolutionFileDetails LoadVisualStudioSolution(string path);
    }
}