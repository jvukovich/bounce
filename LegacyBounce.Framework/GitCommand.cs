using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LegacyBounce.Framework {
    class GitCommand : IGitCommand {
        public void Pull(string workingDirectory, ILog log, IBounce bounce)
        {
            using (new DirectoryChange(workingDirectory)) {
                log.Info("pulling git repo in: " + workingDirectory);
                Git(bounce, "pull");
            }
        }

        public void Clone(string repo, string directory, IDictionary<string, string> options,  ILog log, IBounce bounce)
        {
            log.Info("cloning git repo: {0}, into: {1}", repo, directory);

            if (options == null) {
                Git(bounce, @"clone {0} ""{1}""", repo, directory);
            }
            else {
                Git(bounce, @"clone {0} {1} ""{2}""", options.ToOptionsString(), repo, directory);
            }
        }

        public void Tag(string tag, bool force, IBounce bounce)
        {
            Git(bounce, "tag {0}{1}", force? "-f ": "", tag);
        }

        private void Git(IBounce bounce, string format, params object [] args) {
            Git(bounce, String.Format(format, args));
        }

        private void Git(IBounce bounce, string args) {
            bounce.ShellCommand.ExecuteAndExpectSuccess("cmd", String.Format("/C git {0}", args));
        }

        class DirectoryChange : IDisposable {
            private readonly string OldDirectory;

            public DirectoryChange(string newDir) {
                OldDirectory = Directory.GetCurrentDirectory();
                Directory.SetCurrentDirectory(newDir);
            }

            public void Dispose() {
                Directory.SetCurrentDirectory(OldDirectory);
            }
        }
    }

    static class GitOptionsExtentions {
        public static string ToOptionsString(this IEnumerable<KeyValuePair<string, string>> options) {
            return string.Join("", options.Select(o => string.Format("{0} {1} ", o.Key, o.Value)).ToArray()).Trim();
        }
    }
}