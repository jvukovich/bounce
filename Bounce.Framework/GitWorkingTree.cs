using System.Collections.Generic;
using System.IO;

namespace Bounce.Framework {
    public class GitWorkingTree : ITask {
        public Val<string> Repository;
        public Val<string> Directory;
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
            if (DirectoryUtils.DirectoryExists(WorkingDirectory)) {
                GitCommand.Pull();
            } else {
                GitCommand.Clone(Repository.Value, WorkingDirectory);
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

        public void Clean() {
            DirectoryUtils.DeleteDirectory(WorkingDirectory);
        }

        public Val<string> this[Val<string> filename] {
            get {
                return this.WhenBuilt(() => Path.Combine(WorkingDirectory, filename.Value));
            }
        }
    }
}