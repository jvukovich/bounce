namespace LegacyBounce.Framework {
    public interface ICommandLog {
        void CommandOutput(string output);
        void CommandError(string error);
    }
}