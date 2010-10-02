namespace Bounce.Framework {
    public class GitTag : Task {
        [Dependency] public Val<string> Directory;
        [Dependency] public Val<string> Tag;
        [Dependency] public Val<bool> Force;

        private readonly IGitCommand Git;

        public GitTag() {
            Git = new GitCommand();
            Force = false;
        }

        public override void Build() {
            Git.Tag(Tag.Value, Force.Value);
        }
    }
}