namespace Bounce.Framework {
    public interface IShellCommandExecutor {
        ProcessOutput ExecuteAndExpectSuccess(string commandName, string commandArgs);
        ProcessOutput Execute(string commandName, string commandArgs);
    }
}