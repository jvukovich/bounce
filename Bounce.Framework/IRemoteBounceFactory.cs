namespace Bounce.Framework {
    public interface IRemoteBounceFactory {
        ITask CreateRemoteBounce(Task<string> bounceArguments, Task<string> workingDirectory);
    }
}