namespace Bounce.Framework {
    public interface IShell {
        ProcessOutput Exec(string commandName, string commandArgs, bool allowFailure = false);
        ProcessOutput Exec(string command, bool allowFailure = false);
    }
}