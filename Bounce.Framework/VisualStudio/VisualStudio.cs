namespace Bounce.Framework.VisualStudio
{
    public class VisualStudio : IVisualStudio {
        public IVisualStudioSolution Solution(string path) {
            return new VisualStudioSolution(path);
        }
    }
}
