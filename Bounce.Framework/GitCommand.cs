using System;
using System.IO;

namespace Bounce.Framework {
    class GitCommand : IGitCommand {
        public void Pull(string workingDirectory, ILog log, IBounce bounce)
        {
            using (new DirectoryChange(workingDirectory)) {
                log.Info("pulling git repo in: " + workingDirectory);
                Git(bounce, "pull");
            }
        }

        public void Clone(string repo, string directory, ILog log, IBounce bounce)
        {
            log.Info("cloning git repo: {0}, into: {1}", repo, directory);
            Git(bounce, @"clone {0} ""{1}""", repo, directory);
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
}