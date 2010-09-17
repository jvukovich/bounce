namespace Bounce.Framework {
    public interface IGitRepoParser {
        string ParseCloneDirectoryFromRepoUri(string repoUri);
    }
}