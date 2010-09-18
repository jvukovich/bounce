using System;

namespace Bounce.Framework {
    public class BuildException : BounceException {
        public readonly string Output;

        public BuildException(string message, string output) : base(message) {
            Output = output;
        }

        public override void Explain(System.IO.TextWriter writer) {
            writer.WriteLine(Message);
            writer.Write(Output);
        }
    }
}