using System;
using System.IO;

namespace Bounce.Framework {
    public class GitFiles {
        private readonly ITask gitTask;
        private readonly Func<string> getWorkingDirectory;

        public GitFiles(ITask gitTask, Func<string> checkoutDirectory) {
            this.gitTask = gitTask;
            this.getWorkingDirectory = checkoutDirectory;
        }

        public Val<string> this[Val<string> filename] {
            get {
                return gitTask.WhenBuilt(() => Path.Combine(getWorkingDirectory(), filename.Value));
            }
        }
    }
}