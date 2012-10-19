namespace Bounce.Framework.VisualStudio {
    public interface IVisualStudioSolution : IMsBuildFile {
        string SolutionPath { get; set; }
        VisualStudioProjects Projects { get; }
    }
}