using System.Collections.Generic;

namespace Bounce.Framework {
    public class GitWorkingTree : ITask {
        public IValue<string> Repository;
        public IValue<string> Directory;
        private IGitRepoParser GitRepoParser;
        private IDirectoryUtils DirectoryUtils;
        private readonly IGitCommand GitCommand;

        public GitWorkingTree() : this(new GitRepoParser(), new DirectoryUtils(), new GitCommand()) {}

        public GitWorkingTree(IGitRepoParser gitRepoParser, IDirectoryUtils directoryUtils, IGitCommand gitCommand) {
            GitRepoParser = gitRepoParser;
            DirectoryUtils = directoryUtils;
            GitCommand = gitCommand;
        }

        public IEnumerable<ITask> Dependencies {
            get { return new[] {Repository, Directory}; }
        }

        public void BeforeBuild() {
        }

        public void Build() {
            if (DirectoryUtils.DirectoryExists(OptionalPath)) {
                GitCommand.Pull();
            } else {
                GitCommand.Clone(Repository.Value, OptionalPath);
            }
        }

        private string OptionalPath {
            get {
                if (Directory != null && Directory.Value != null) {
                    return Directory.Value;
                } else {
                    return GitRepoParser.ParseCloneDirectoryFromRepoUri(Repository.Value);
                }
            }
        }

        public void Clean() {
            DirectoryUtils.DeleteDirectory(OptionalPath);
        }
    }
}