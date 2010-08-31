using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace Bounce {
    public class SourceDirectory : ISourceDirectory {
        public IEnumerable<ITarget> Dependencies {
            get { return new ITarget[0]; }
        }

        public void Build () {
            if (!LastBuilt.HasValue) {
                LastBuilt = DateTime.UtcNow;
            }
        }

        public void Clean () {}

        public DateTime? LastBuilt { get; set; }

        public IValue<string> Path { get; set; }
    }
}