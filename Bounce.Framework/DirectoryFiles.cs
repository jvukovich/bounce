using System;
using System.IO;

namespace Bounce.Framework {
    public class DirectoryFiles {
        private readonly Task<string> RootDirectory;

        public DirectoryFiles(Task<string> rootDirectory) {
            RootDirectory = rootDirectory;
        }

        public Task<string> this[Task<string> filename] {
            get {
                return new All(RootDirectory, filename).WhenBuilt(() => Path.Combine(RootDirectory.Value, filename.Value));
            }
        }
    }
}