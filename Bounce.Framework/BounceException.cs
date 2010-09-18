using System;
using System.IO;

namespace Bounce.Framework {
    public class BounceException : Exception {
        public BounceException(string message) : base(message) {
        }

        public virtual void Explain(TextWriter writer) {
            writer.WriteLine(this);
        }
    }
}