using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace Bounce.Framework {
    public class GitCheckout : Task {
        [Dependency]
        public Task<string> Repository;
        [Dependency]
        public Task<string> Directory;
        [Dependency]
        public Task<string> Branch;

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
                var options = GetCommandOptions();
                GitCommand.Clone(Repository.Value, WorkingDirectory, options, bounce.Log, bounce);
            }
        }

        private IDictionary<string, string> GetCommandOptions() {
            if (Branch != null && Branch.Value != null) {
                return new Dictionary<string, string> { { "--branch", Branch.Value } };
            }

            return null;
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
                return new DirectoryFiles(this.WhenBuilt(() => WorkingDirectory));
            }
        }
    }
}