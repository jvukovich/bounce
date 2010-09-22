namespace Bounce.Framework {
    public interface IShellCommandExecutor {
        void ExecuteAndExpectSuccess(string commandName, string commandArgs);
        ProcessOutput Execute(string commandName, string commandArgs);
    }
}