using ICSharpCode.SharpZipLib.Zip;

namespace LegacyBounce.Framework {
    class ZipFileCreator : IZipFileCreator {
        public void CreateZipFile(string zipFileName, string directory) {
            new FastZip().CreateZip(zipFileName, directory, true, null);
        }
    }
}