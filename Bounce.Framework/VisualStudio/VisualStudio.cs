namespace Bounce.Framework.VisualStudio
{
    public class VisualStudio : IVisualStudio {
        public IMsBuild MsBuild { get; private set; }

        public VisualStudio() {
            MsBuild = new MsBuild();
        }

        public IVisualStudioSolution Solution(string path) {
            var sln = new VisualStudioSolutionFileReader().ReadSolution(path, "Debug");
            sln.MsBuild = MsBuild;
            return sln;
        }
    }
}
