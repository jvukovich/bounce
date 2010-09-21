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
                ShellCommandExecutor.ExecuteAndExpectSuccess("cmd", @"/C git pull");
            }
        }

        public void Clone(string repo, string directory, ILog log) {
            log.Info("cloning git repo: {0}, into: {1}", repo, directory);
            ShellCommandExecutor.ExecuteAndExpectSuccess("cmd", String.Format(@"/C git clone {0} ""{1}""", repo, directory));
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