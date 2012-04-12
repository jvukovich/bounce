using System.IO;

namespace Bounce.Framework.Obsolete {
    public class CommandExecutionException : BounceException {
        public readonly string Output;

        public CommandExecutionException(string message, string output) : base(message) {
            Output = output;
        }

        public override void Explain(TextWriter stderr) {
            stderr.WriteLine(Message);
            stderr.WriteLine(Output);
        }
    }
}