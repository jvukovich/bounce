namespace Bounce.Framework.VisualStudio
{
    public interface IVisualStudioSolution : IMsBuildFile
    {
        string SolutionPath { get; }
        VisualStudioProjects Projects { get; }
    }
}