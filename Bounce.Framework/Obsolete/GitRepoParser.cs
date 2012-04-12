using System.IO;

namespace Bounce.Framework.Obsolete {
    public class GitRepoParser : IGitRepoParser {
        public string ParseCloneDirectoryFromRepoUri(string repoUri) {
            repoUri = repoUri.TrimEnd('/', '\\');

            if (Path.GetExtension(repoUri) == ".git") {
                string withoutGit = Path.GetFileNameWithoutExtension(repoUri);

                if (withoutGit != "") {
                    return withoutGit;
                } else {
                    return Path.GetFileName(Path.GetDirectoryName(repoUri));
                }
            } else {
                return Path.GetFileName(repoUri);
            }
        }
    }
}