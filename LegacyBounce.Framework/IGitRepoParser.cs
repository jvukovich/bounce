namespace LegacyBounce.Framework {
    public interface IGitRepoParser {
        string ParseCloneDirectoryFromRepoUri(string repoUri);
    }
}