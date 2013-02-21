namespace Bounce.Framework.VisualStudio
{
    public class VisualStudio : IVisualStudio {
        public IMsBuild MsBuild { get; private set; }

        public VisualStudio(IShell shell) {
            MsBuild = new MsBuild(shell);
        }

        public IVisualStudioSolution Solution(string path) {
            var sln = new VisualStudioSolutionFileReader().ReadSolution(path, "Debug");
            sln.MsBuild = MsBuild;
            return sln;
        }
    }
}
