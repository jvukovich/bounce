namespace Bounce.Framework {
    public class GitTag : Task {
        [Dependency] public Future<string> Directory;
        [Dependency] public Future<string> Tag;
        [Dependency] public Future<bool> Force;

        private readonly IGitCommand Git;

        public GitTag() {
            Git = new GitCommand();
            Force = false;
        }

        public override void Build(IBounce bounce) {
            Git.Tag(Tag.Value, Force.Value, bounce);
        }
    }
}