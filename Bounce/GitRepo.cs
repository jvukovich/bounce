using System.Collections.Generic;

namespace Bounce.Framework {
    public class GitRepo : ITask {
        public IValue<string> Origin;
        public IValue<string> Path;
        private IGitRepoParser GitRepoParser;
        private IDirectoryUtils DirectoryUtils;
        private IShellCommandExecutor ShellCommandExecutor;

        public GitRepo() : this(new GitRepoParser(), new DirectoryUtils(), new ShellCommandExecutor()) {}

        public GitRepo(IGitRepoParser gitRepoParser, IDirectoryUtils directoryUtils, IShellCommandExecutor shellCommandExecutor) {
            GitRepoParser = gitRepoParser;
            DirectoryUtils = directoryUtils;
            ShellCommandExecutor = shellCommandExecutor;
        }

        public IEnumerable<ITask> Dependencies {
            get { return new[] {Origin, Path}; }
        }

        public void BeforeBuild() {
        }

        public void Build() {
//            ShellCommandExecutor.ExecuteAndExpectSuccess("git", string.Format(@"clone {0}{1}", Origin.Value, OptionalPath));
        }

        private object OptionalPath {
            get {
                if (Path != null || Path.Value != null) {
                    return string.Format(@" ""{0}""", Path.Value);
                } else {
                    return "";
                }
            }
        }

        public void Clean() {
//            var dir = GitRepoParser.ParseCloneDirectoryFromRepoUri(Origin.Value);
//            DirectoryUtils.DeleteDirectory(dir);
        }
    }
}