namespace Bounce.Framework.Obsolete {
    public interface IShellCommandExecutor {
        ProcessOutput ExecuteAndExpectSuccess(string commandName, string commandArgs);
        ProcessOutput Execute(string commandName, string commandArgs);
    }
}