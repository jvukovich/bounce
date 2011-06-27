using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Bounce.Framework {
    public class DirectoryUtils : IDirectoryUtils {
        private readonly IFileNameFilterFactory FileNameFilterFactory;

        public DirectoryUtils() : this(new FileNameFilterFactory()) {}

        public DirectoryUtils(IFileNameFilterFactory fileNameFilterFactory) {
            FileNameFilterFactory = fileNameFilterFactory;
        }

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

        private static void CopyDirectoryContents(string originalFrom, string from, string to, IFileNameFilter fileNameFilter) {
            if (!Directory.Exists(to)) {
                Directory.CreateDirectory(to);
            }

            foreach(var file in Directory.GetFiles(from)) {
                var destFilename = Path.Combine(to, Path.GetFileName(file));

                if (fileNameFilter.IncludeFile(file.Substring(originalFrom.Length + 1))) {
                    File.Copy(file, destFilename);
                }
            }

            foreach(var subdir in Directory.GetDirectories(from)) {
                if (fileNameFilter.IncludeFile(subdir.Substring(originalFrom.Length + 1) + @"\")) {
                    CopyDirectoryContents(originalFrom, subdir, Path.Combine(to, Path.GetFileName(subdir)),
                                          fileNameFilter);
                }
            }
        }

        public void DeleteDirectory(string dir) {
            if (Directory.Exists(dir)) {
                try
                {
                    Directory.Delete(dir, true);
                }
                catch(Exception ex)
                {
                    throw new IOException(string.Format("Failed to delete {0}. {1}", dir, ex.Message), ex);
                }
            }
        }

        public void DeleteDirectoryContents(string dir) {
            if (Directory.Exists(dir)) {
                foreach(var path in Directory.GetFiles(dir)) {
                    File.Delete(path);
                }

                foreach(var path in Directory.GetDirectories(dir)) {
                    DeleteDirectory(path);
                }
            }
        }

        public bool DirectoryExists(string dir) {
            return Directory.Exists(dir);
        }

        public void CopyDirectory(string from, string to, IEnumerable<string> excludes, IEnumerable<string> includes) {
            CopyDirectoryContents(from, from, to, FileNameFilterFactory.CreateFileNameFilter(excludes, includes));
        }

        public void CreateDirectory(string path) {
            if (String.IsNullOrEmpty(path)) {
                return;
            }

            var parentDirectory = Path.GetDirectoryName(path);
            if (Directory.Exists(parentDirectory)) {
                CreateDirectory(parentDirectory);
            }

            Directory.CreateDirectory(path);
        }
    }
}