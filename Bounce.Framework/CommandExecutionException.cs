using System.IO;

namespace Bounce.Framework
{
    public class CommandExecutionException : BounceException
    {
        public readonly string Output;
        public readonly int ExitCode;

        public CommandExecutionException(string message, string output, int exitCode) : base(message)
        {
            Output = output;
            ExitCode = exitCode;
        }

        public override void Explain(TextWriter stderr)
        {
            stderr.WriteLine(Message);
            stderr.WriteLine(Output);
        }
    }
}