namespace Bounce.Framework {
    public interface IGitCommand {
        void Pull(string workingDirectory, ILog log);
        void Clone(string repo, string directory, ILog log);
    }
}