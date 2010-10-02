using System;
using System.IO;

namespace Bounce.Framework {
    class GitCommand : IGitCommand {
        private readonly IShellCommandExecutor ShellCommandExecutor;

        public GitCommand() {
            ShellCommandExecutor = new ShellCommandExecutor();
        }

        public void Pull(string workingDirectory, ILog log) {
            using (new DirectoryChange(workingDirectory)) {
                log.Info("pulling git repo in: " + workingDirectory);
                Git("pull");
            }
        }

        public void Clone(string repo, string directory, ILog log) {
            log.Info("cloning git repo: {0}, into: {1}", repo, directory);
            Git(@"clone {0} ""{1}""", repo, directory);
        }

        public void Tag(string tag, bool force) {
            Git("tag {0}{1}", force? "-f ": "", tag);
        }

        private void Git(string format, params object [] args) {
            Git(String.Format(format, args));
        }

        private void Git(string args) {
            ShellCommandExecutor.ExecuteAndExpectSuccess("cmd", String.Format("/C git {0}", args));
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