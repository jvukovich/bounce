using System;
using ICSharpCode.SharpZipLib.Zip;

namespace Bounce.Framework {
    public interface IZipFileCreator {
        void CreateZipFile(string zipFileName, string directory);
    }

    class ZipFileCreator : IZipFileCreator {
        public void CreateZipFile(string zipFileName, string directory) {
            new FastZip().CreateZip(zipFileName, directory, true, null);
        }
    }
}