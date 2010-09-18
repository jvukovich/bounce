namespace Bounce.Framework {
    public interface IGitCommand {
        void Pull();
        void Clone(string repo, string directory);
    }
}