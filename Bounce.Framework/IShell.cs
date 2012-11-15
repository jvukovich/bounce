namespace Bounce.Framework {
    public interface IShell {
        ProcessOutput ExecuteAndExpectSuccess(string commandName, string commandArgs);
        ProcessOutput Execute(string commandName, string commandArgs);
    }
}