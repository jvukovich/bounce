using System;
using System.IO;

namespace Bounce.Console {
    public class BeforeBounceScriptException : BounceConsoleException {
        private string ScriptPath;
        private readonly string ScriptOutput;
        private readonly int ExitCode;

        public BeforeBounceScriptException(string scriptPath, string scriptOutput, int exitCode) {
            ScriptPath = scriptPath;
            ScriptOutput = scriptOutput;
            ExitCode = exitCode;
        }

        public override void Explain(TextWriter writer) {
            writer.WriteLine(String.Format(@"{0} failed", ScriptPath));
            writer.WriteLine("exit code: " + ExitCode);
            writer.Write(ScriptOutput);
        }
    }
}