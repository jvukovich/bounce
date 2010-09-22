namespace Bounce.Framework {
    class NullCommandLog : ICommandLog {
        public void CommandOutput(string output) {
        }

        public void CommandError(string error) {
        }

        public void CommandComplete(int exitCode) {
        }
    }
}