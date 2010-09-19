using System.IO;

namespace Bounce.Framework {
    public class ZipFile : Task {
        private readonly IZipFileCreator ZipFileCreator;
        private readonly IFileUtils FileUtils;
        private readonly IDirectoryUtils DirectoryUtils;

        [Dependency]
        public Val<string> Directory;
        [Dependency]
        public Val<string> ZipFileName;

        public ZipFile() : this(new ZipFileCreator(), new FileUtils(), new DirectoryUtils()) {
        }

        public ZipFile(IZipFileCreator zipFileCreator, IFileUtils fileUtils, IDirectoryUtils directoryUtils) {
            ZipFileCreator = zipFileCreator;
            FileUtils = fileUtils;
            DirectoryUtils = directoryUtils;
        }

        public override void Build() {
            if (!FileUtils.FileExists(ZipFileName.Value) || (FileUtils.LastWriteTimeForFile(ZipFileName.Value) < DirectoryUtils.GetLastModTimeForDirectory(Directory.Value))) {
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