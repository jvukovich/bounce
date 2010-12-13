using System;
using System.Collections.Generic;
using System.IO;

namespace Bounce.Framework {
    public class Copy : Task {
        private readonly IFileSystemCopier FileSystemCopier;

        public Copy() : this(new FileSystemCopier()) {}

        public Copy(IFileSystemCopier fileSystemCopier) {
            FileSystemCopier = fileSystemCopier;
        }

        /// <summary>
        /// A file or directory to copy from
        /// </summary>
        [Dependency, CleanAfterBuild]
        public Future<string> FromPath { get; set; }

        [Dependency]
        private Future<string> _toPath;

        /// <summary>
        /// A file or directory to copy to
        /// </summary>
        public Future<string> ToPath
        {
            get { return this.WhenBuilt(() => _toPath.Value); }
            set { _toPath = value; }
        }

        /// <summary>
        /// Glob patterns of files and directories not to copy
        /// </summary>
        [Dependency]
        public Future<IEnumerable<string>> Excludes;
        /// <summary>
        /// Glob patterns of files and directories to copy, overriding Excludes
        /// </summary>
        [Dependency]
        public Future<IEnumerable<string>> Includes;

        public override void Build(IBounce bounce) {
            var fromPath = FromPath.Value;
            var toPath = ToPath.Value;

            bounce.Log.Debug(Directory.GetCurrentDirectory());
            bounce.Log.Debug("copying from: {0}, to: {1}", FromPath.Value, ToPath.Value);

            FileSystemCopier.Copy(fromPath, toPath, GetValueOf(Excludes), GetValueOf(Includes));
        }

        private IEnumerable<string> GetValueOf(Future<IEnumerable<string>> paths) {
            return paths != null ? paths.Value : null;
        }

        public override void Clean() {
            FileSystemCopier.Delete(ToPath.Value);
        }
    }
}