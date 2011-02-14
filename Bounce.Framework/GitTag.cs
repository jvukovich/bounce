namespace Bounce.Framework {
    public class GitTag : Task {
        [Dependency] public Task<string> Directory;
        [Dependency] public Task<string> Tag;
        [Dependency] public Task<bool> Force;

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