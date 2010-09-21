using System.IO;

namespace Bounce.Framework {
    public interface ICommandLog {
        void CommandOutput(string output);
        void CommandError(string error);
        void CommandComplete(int exitCode);
    }

    class CommandLog : ICommandLog {
        private readonly string Command;
        private readonly TextWriter Stdout;
        private readonly TextWriter Stderr;

        public CommandLog(string command, string args, TextWriter stdout, TextWriter stderr) {
            Command = command;
            Stderr = stderr;
            Stdout = stdout;

            Stdout.WriteLine("in: {0}", Directory.GetCurrentDirectory());
            Stdout.WriteLine("exec: {0} {1}", command, args);
        }

        public void CommandOutput(string output) {
            Stdout.Write(output);
        }

        public void CommandError(string error) {
            Stderr.Write(error);
        }

        public void CommandComplete(int exitCode) {
            Stdout.WriteLine();
            Stdout.WriteLine("command: {0}, complete with: {1}", Command, exitCode);
        }
    }
}