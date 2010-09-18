using System;

namespace Bounce.Framework {
    class GitCommand : IGitCommand {
        private readonly IShellCommandExecutor ShellCommandExecutor;

        public GitCommand() {
            ShellCommandExecutor = new ShellCommandExecutor();
        }

        public void Pull() {
            ShellCommandExecutor.ExecuteAndExpectSuccess("git", "pull");
        }

        public void Clone(string repo, string directory) {
            ShellCommandExecutor.ExecuteAndExpectSuccess("git", String.Format(@"clone {0} ""{1}""", repo, directory));
        }
    }
}