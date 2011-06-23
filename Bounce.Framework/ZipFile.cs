using System.IO;

namespace Bounce.Framework {
    public class ZipFile : Task {
        private readonly IZipFileCreator ZipFileCreator;
        private readonly IFileUtils FileUtils;
        private readonly IDirectoryUtils DirectoryUtils;

        [Dependency]
        public Task<string> Directory;
        [Dependency]
        public Task<string> ZipFileName;

        public ZipFile() : this(new ZipFileCreator(), new FileUtils(), new DirectoryUtils()) {
        }

        public ZipFile(IZipFileCreator zipFileCreator, IFileUtils fileUtils, IDirectoryUtils directoryUtils) {
            ZipFileCreator = zipFileCreator;
            FileUtils = fileUtils;
            DirectoryUtils = directoryUtils;
        }

        public override void Build() {
            var zipFileName = ZipFileName.Value;
            var directory = Directory.Value;

            if (!FileUtils.FileExists(zipFileName) || (FileUtils.LastWriteTimeForFile(zipFileName) < DirectoryUtils.GetLastModTimeForDirectory(directory))) {
                FileUtils.DeleteFile(zipFileName);
                ZipFileCreator.CreateZipFile(zipFileName, directory);
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