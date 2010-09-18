using System;

namespace Bounce.Framework {
    public class BuildException : Exception {
        public readonly string Output;

        public BuildException(string message, string output) : base(message) {
            Output = output;
        }
    }
}