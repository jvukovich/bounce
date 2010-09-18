using System;
using System.Collections.Generic;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;

namespace Bounce.Framework {
    public class ZipFile : Task {
        private readonly IZipFileCreator ZipFileCreator;
        private readonly IFileSystem FileSystem;
        private readonly IDirectoryUtils DirectoryUtils;

        [Dependency]
        public Val<string> Directory;
        [Dependency]
        public Val<string> ZipFileName;

        public ZipFile() : this(new ZipFileCreator(), new FileSystem(), new DirectoryUtils()) {
        }

        public ZipFile(IZipFileCreator zipFileCreator, IFileSystem fileSystem, IDirectoryUtils directoryUtils) {
            ZipFileCreator = zipFileCreator;
            FileSystem = fileSystem;
            DirectoryUtils = directoryUtils;
        }

        public override void Build() {
            if (!FileSystem.FileExists(ZipFileName.Value) || (FileSystem.LastWriteTimeForFile(ZipFileName.Value) < DirectoryUtils.GetLastModTimeForDirectory(Directory.Value))) {
                ZipFileCreator.CreateZipFile(ZipFileName.Value, Directory.Value);
            }
        }

        public override void Clean() {
            var filename = ZipFileName.Value;
            if (File.Exists(filename)) {
                File.Delete(filename);
            }
        }
    }
}