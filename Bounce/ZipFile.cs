using System;
using System.Collections.Generic;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;

namespace Bounce.Framework {
    public class ZipFile : ITarget {
        public IValue<string> Directory;
        public IValue<string> ZipFileName;

        public IEnumerable<ITarget> Dependencies {
            get { return new[] {Directory, ZipFileName}; }
        }

        public void BeforeBuild() {
        }

        public void Build() {
            new FastZip().CreateZip(ZipFileName.Value, Directory.Value, true, null);
        }

        public void Clean() {
            var filename = ZipFileName.Value;
            if (File.Exists(filename)) {
                File.Delete(filename);
            }
        }
    }
}