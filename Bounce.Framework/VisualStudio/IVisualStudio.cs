namespace Bounce.Framework.VisualStudio
{
    public interface IVisualStudio
    {
        IMsBuild MsBuild { get; }
        IVisualStudioSolution Solution(string path);
    }
}