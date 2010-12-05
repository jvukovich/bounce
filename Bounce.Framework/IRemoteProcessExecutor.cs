namespace Bounce.Framework {
    public interface IRemoteProcessExecutor {
        void ExecuteRemoteProcess(string command, string arguments);
    }
}