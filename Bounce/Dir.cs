using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Bounce.Framework {
    public class Dir : ITarget {
        public IEnumerable<ITarget> Dependencies {
            get { return new[] {Path}; }
        }

        public DateTime? LastBuilt {
            get { return DirectoryUtils.GetLastModTimeForFilesInDirectory(Path.Value); }
        }

        public IValue<string> Path { get; set; }

        public void Build() {
            throw new NotImplementedException();
        }

        public void Clean() {
            throw new NotImplementedException();
        }
    }

    public class DirectoryUtils {
        public static DateTime GetLastModTimeForFilesInDirectory(string dir) {
            var modTimes = new List<DateTime>();
            modTimes.Add(Directory.GetLastWriteTimeUtc(dir));

            foreach (var file in Directory.GetFiles(dir))
            {
                modTimes.Add(File.GetLastWriteTimeUtc(file));
            }

            foreach (var subdir in Directory.GetDirectories(dir))
            {
                modTimes.Add(GetLastModTimeForFilesInDirectory(subdir));
            }

            return modTimes.Max();
        }

        public static void CopyDirectoryContents(string from, string to) {
            string fullFromDir = Path.GetFullPath(from);
            string fullToDir = Path.GetFullPath(to);

            if (!Directory.Exists(fullToDir)) {
                Directory.CreateDirectory(fullToDir);
            }

            foreach(var file in Directory.GetFiles(fullFromDir)) {
                var destFilename = Path.Combine(fullToDir, file.Substring(fullFromDir.Length + 1));
                File.Copy(file, destFilename);
            }
            foreach(var subdir in Directory.GetDirectories(from)) {
                CopyDirectoryContents(subdir, Path.Combine(to, Path.GetFileName(subdir)));
            }
        }
    }

    public class ToDir : ITarget {
        public IValue<string> FromPath { get; set; }
        public IValue<string> ToPath { get; set; }

        public IEnumerable<ITarget> Dependencies {
            get { throw new NotImplementedException(); }
        }

        public DateTime? LastBuilt {
            get { return DirectoryUtils.GetLastModTimeForFilesInDirectory(FromPath.Value); }
        }

        public void Build() {
            DirectoryUtils.CopyDirectoryContents(FromPath.Value, ToPath.Value);
        }

        public void Clean() {
            var toPath = ToPath.Value;

            if (Directory.Exists(toPath)) {
                Directory.Delete(toPath, true);
            }
        }
    }
}