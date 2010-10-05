namespace Bounce.Framework {
    class NullCommandLog : ICommandLog {
        public NullCommandLog(string commandArgumentsForLogging) {
            CommandArgumentsForLogging = commandArgumentsForLogging;
        }

        public void CommandOutput(string output) {
        }

        public void CommandError(string error) {
        }

        public void CommandComplete(int exitCode) {
        }

        public string CommandArgumentsForLogging { get; private set; }
    }
}