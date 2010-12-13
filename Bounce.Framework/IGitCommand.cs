namespace Bounce.Framework {
    public interface IGitCommand {
        void Pull(string workingDirectory, ILog log, IBounce bounce);
        void Clone(string repo, string directory, ILog log, IBounce bounce);
        void Tag(string tag, bool force, IBounce bounce);
    }
}