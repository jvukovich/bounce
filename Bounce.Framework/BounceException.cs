using System;
using System.IO;

namespace Bounce.Framework {
    [Serializable]
    public class BounceException : Exception {
        public BounceException(string message) : base(message) {
        }

        public BounceException(string message, Exception innerException) : base(message, innerException) {
        }

        public BounceException() {
        }

        public virtual void Explain(TextWriter stderr) {
            stderr.WriteLine(this);
        }
    }
}