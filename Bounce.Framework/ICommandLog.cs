namespace Bounce.Framework
{
    public interface ICommandLog
    {
        void CommandOutput(string output);
        void CommandError(string error);
        void CommandComplete(int exitCode);
        string CommandArgumentsForLogging { get; }
    }
}