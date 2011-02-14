using System;
using System.IO;

namespace Bounce.Framework {
    public class GitCheckout : Task {
        [Dependency]
        public Task<string> Repository;
        [Dependency]
        public Task<string> Directory;

        private IGitRepoParser GitRepoParser;
        private IDirectoryUtils DirectoryUtils;
        private readonly IGitCommand GitCommand;

        public GitCheckout() : this(new GitRepoParser(), new DirectoryUtils(), new GitCommand()) {}

        public GitCheckout(IGitRepoParser gitRepoParser, IDirectoryUtils directoryUtils, IGitCommand gitCommand) {
            GitRepoParser = gitRepoParser;
            DirectoryUtils = directoryUtils;
            GitCommand = gitCommand;
        }

        public override void Build(IBounce bounce) {
            bounce.Log.Debug("pwd");
            bounce.Log.Debug(System.IO.Directory.GetCurrentDirectory());
            if (DirectoryUtils.DirectoryExists(WorkingDirectory)) {
                GitCommand.Pull(WorkingDirectory, bounce.Log, bounce);
            } else {
                GitCommand.Clone(Repository.Value, WorkingDirectory, bounce.Log, bounce);
            }
        }

        private string WorkingDirectory {
            get {
                if (Directory != null && Directory.Value != null) {
                    return Directory.Value;
                } else {
                    return GitRepoParser.ParseCloneDirectoryFromRepoUri(Repository.Value);
                }
            }
        }

        public override void Clean() {
            DirectoryUtils.DeleteDirectory(WorkingDirectory);
        }

        public DirectoryFiles Files {
            get {
                return new DirectoryFiles(this, () => WorkingDirectory);
            }
        }
    }
}