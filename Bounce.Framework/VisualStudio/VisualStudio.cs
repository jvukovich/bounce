namespace Bounce.Framework.VisualStudio
{
    public class VisualStudio
    {
        public VisualStudioSolution Solution(string path) {
            return new VisualStudioSolution(path);
        }
    }
}
