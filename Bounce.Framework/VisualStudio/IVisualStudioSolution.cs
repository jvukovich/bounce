namespace Bounce.Framework.VisualStudio {
    public interface IVisualStudioSolution {
        string MsBuildExe { get; set; }
        string SolutionPath { get; set; }
        string Configuration { get; set; }
        string OutputDir { get; set; }
        string Target { get; set; }
        VisualStudioProjects Projects { get; }
        void Build();
    }
}