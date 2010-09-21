using System;
using System.IO;

namespace Bounce.Framework {
    public class DirectoryFiles {
        private readonly ITask Task;
        private readonly Func<string> RootDirectory;

        public DirectoryFiles(ITask task, Func<string> rootDirectory) {
            Task = task;
            RootDirectory = rootDirectory;
        }

        public Val<string> this[Val<string> filename] {
            get {
                return Task.WhenBuilt(() => Path.Combine(RootDirectory(), filename.Value));
            }
        }
    }
}