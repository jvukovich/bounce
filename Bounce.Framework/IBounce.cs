namespace Bounce.Framework {
    public interface IBounce {
        ILog Log { get; }
        IShellCommandExecutor ShellCommand { get; }
    }
}