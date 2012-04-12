using System;
using System.Collections.Generic;
using System.IO;

namespace LegacyBounce.Framework {
    public class FileSystemCopier : IFileSystemCopier {
        private IFileUtils FileUtils;
        private IDirectoryUtils DirectoryUtils;

        public FileSystemCopier() : this(new FileUtils(), new DirectoryUtils()) {}

        public FileSystemCopier(IFileUtils fileUtils, IDirectoryUtils directoryUtils) {
            FileUtils = fileUtils;
            DirectoryUtils = directoryUtils;
        }

        public DateTime GetLastModTimeForPath(string path) {
            if (DirectoryUtils.DirectoryExists(path)) {
                return DirectoryUtils.GetLastModTimeForDirectory(path);
            } else if (FileUtils.FileExists(path)) {
                return FileUtils.LastWriteTimeForFile(path);
            } else {
                throw new FileNotFoundException(String.Format("file not found `{0}'", path));
            }
        }

        public void Copy(string from, string to, IEnumerable<string> excludes, IEnumerable<string> includes, bool deleteToDirectory) {
            if (DirectoryUtils.DirectoryExists(from)) {
                if (deleteToDirectory)
                {
                    DirectoryUtils.DeleteDirectoryContents(to);
                }
                DirectoryUtils.CopyDirectory(from, to, excludes, includes);
            } else {
                string dest;

                if (DirectoryUtils.DirectoryExists(to)) {
                    dest = Path.Combine(to, Path.GetFileName(from));
                } else
                {
                    DirectoryUtils.CreateDirectory(Path.GetDirectoryName(to));
                    dest = to;
                }
                FileUtils.CopyFile(from, dest);
            }
        }

        public void Delete(string path) {
            if (DirectoryUtils.DirectoryExists(path)) {
                DirectoryUtils.DeleteDirectory(path);
            } else if (FileUtils.FileExists(path)) {
                FileUtils.DeleteFile(path);
            } else {
                throw new FileNotFoundException(String.Format("file not found `{0}'", path));
            }
        }

        public bool Exists(string path) {
            return FileUtils.FileExists(path) || DirectoryUtils.DirectoryExists(path);
        }
    }
}