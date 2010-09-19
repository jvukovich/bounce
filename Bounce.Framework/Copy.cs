using System.Collections.Generic;

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

        public override void Build() {
            var fromPath = FromPath.Value;
            var toPath = ToPath.Value;

            if (!FileSystemCopier.Exists(toPath) || FileSystemCopier.GetLastModTimeForPath(fromPath) > FileSystemCopier.GetLastModTimeForPath(toPath)) {
                FileSystemCopier.Copy(fromPath, toPath, Excludes.Value, Includes.Value);
            }
        }

        public override void Clean() {
            FileSystemCopier.Delete(ToPath.Value);
        }
    }

    public class AspNetWebSiteDirectory : Copy {
        public AspNetWebSiteDirectory() {
            Excludes = new[] {
                "**.cs",
                @"obj\**",
                "**.csproj.user",
                "**.csproj",
                "**.pdb",
            };
        }
    }
}