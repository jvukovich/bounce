using System.Collections.Generic;

namespace Bounce.Framework {
    public class GitRepo : ITask {
        public IValue<string> Origin;
        public IValue<string> Path;
        private IGitRepoParser GitRepoParser;
        private IDirectoryUtils DirectoryUtils;
        private readonly IGitCommand GitCommand;

        public GitRepo() : this(new GitRepoParser(), new DirectoryUtils(), new GitCommand()) {}

        public GitRepo(IGitRepoParser gitRepoParser, IDirectoryUtils directoryUtils, IGitCommand gitCommand) {
            GitRepoParser = gitRepoParser;
            DirectoryUtils = directoryUtils;
            GitCommand = gitCommand;
        }

        public IEnumerable<ITask> Dependencies {
            get { return new[] {Origin, Path}; }
        }

        public void BeforeBuild() {
        }

        public void Build() {
            if (DirectoryUtils.DirectoryExists(OptionalPath)) {
                GitCommand.Pull();
            } else {
                GitCommand.Clone(Origin.Value, OptionalPath);
            }
        }

        private string OptionalPath {
            get {
                if (Path != null && Path.Value != null) {
                    return Path.Value;
                } else {
                    return GitRepoParser.ParseCloneDirectoryFromRepoUri(Origin.Value);
                }
            }
        }

        public void Clean() {
            DirectoryUtils.DeleteDirectory(OptionalPath);
        }
    }
}