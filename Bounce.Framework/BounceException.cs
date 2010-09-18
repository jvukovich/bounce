using System;

namespace Bounce.Framework {
    internal class BounceException : Exception {
        public BounceException(string message) : base(message) {
        }
    }
}