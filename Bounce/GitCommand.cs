using System;

namespace Bounce.Framework {
    class GitCommand : IGitCommand {
        private readonly IShellCommandExecutor ShellCommandExecutor;

        public GitCommand() {
            ShellCommandExecutor = new ShellCommandExecutor();
        }

        public void Pull() {
            Console.WriteLine("pulling git repo");
            var output = ShellCommandExecutor.Execute("cmd", @"/C git pull");
            Console.WriteLine(output.ExitCode);
            Console.WriteLine(output.ErrorAndOutput);
        }

        public void Clone(string repo, string directory) {
            Console.WriteLine("cloning git repo: {0}, into: {1}", repo, directory);
            var output = ShellCommandExecutor.Execute("cmd", String.Format(@"/C git clone {0} ""{1}""", repo, directory));
            Console.WriteLine(output.ExitCode);
            Console.WriteLine(output.ErrorAndOutput);
        }
    }
}