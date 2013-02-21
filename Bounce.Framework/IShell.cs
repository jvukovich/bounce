namespace Bounce.Framework {
    public interface IShell {
        ProcessOutput Exec(string commandName, string commandArgs, IShellLogger logger = null);
        ProcessOutput Exec(string command, IShellLogger logger = null);
    }
}