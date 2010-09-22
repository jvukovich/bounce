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

        [Dependency]
        public Val<string> FromPath { get; set; }
        [Dependency]
        public Val<string> ToPath { get; set; }
        [Dependency]
        public Val<IEnumerable<string>> Excludes;
        [Dependency]
        public Val<IEnumerable<string>> Includes;

        public override void Build(IBounce bounce) {
            var fromPath = FromPath.Value;
            var toPath = ToPath.Value;

            bounce.Log.Debug(Directory.GetCurrentDirectory());
            bounce.Log.Debug("copying from: {0}, to: {1}", FromPath.Value, ToPath.Value);

            FileSystemCopier.Copy(fromPath, toPath, GetValueOf(Excludes), GetValueOf(Includes));
        }

        private IEnumerable<string> GetValueOf(Val<IEnumerable<string>> paths) {
            return paths != null ? paths.Value : null;
        }

        public override void Clean() {
            FileSystemCopier.Delete(ToPath.Value);
        }
    }
}