namespace Bounce.Framework {
    public interface IGitCommand {
        void Pull(string workingDirectory);
        void Clone(string repo, string directory);
    }
}