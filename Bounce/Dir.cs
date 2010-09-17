using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Bounce.Framework {
    public class DirectoryUtils : IDirectoryUtils {
        public DateTime GetLastModTimeForDirectory(string dir) {
            var modTimes = new List<DateTime>();
            modTimes.Add(Directory.GetLastWriteTimeUtc(dir));

            foreach (var file in Directory.GetFiles(dir))
            {
                modTimes.Add(File.GetLastWriteTimeUtc(file));
            }

            foreach (var subdir in Directory.GetDirectories(dir))
            {
                modTimes.Add(GetLastModTimeForDirectory(subdir));
            }

            return modTimes.Max();
        }

        public void CopyDirectoryContents(string from, string to) {
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

        public void DeleteDirectory(string dir) {
            if (Directory.Exists(dir)) {
                Directory.Delete(dir, true);
            }
        }
    }

    public class ToDir : ITarget {
        private readonly IDirectoryUtils DirUtils;

        public ToDir() : this(new DirectoryUtils()) {}

        public ToDir(IDirectoryUtils dirUtils) {
            DirUtils = dirUtils;
        }

        public IValue<string> FromPath { get; set; }
        public IValue<string> ToPath { get; set; }

        public IEnumerable<ITarget> Dependencies {
            get { return new[] {FromPath, ToPath}; }
        }

        public void BeforeBuild() {
        }

        public void Build() {
            var fromPath = FromPath.Value;
            var toPath = ToPath.Value;

            if (DirUtils.GetLastModTimeForDirectory(fromPath) > DirUtils.GetLastModTimeForDirectory(toPath)) {
                DirUtils.CopyDirectoryContents(fromPath, toPath);
            }
        }

        public void Clean() {
            DirUtils.DeleteDirectory(ToPath.Value);
        }
    }
}