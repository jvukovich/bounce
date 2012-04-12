namespace Bounce.Framework.Obsolete {
    public interface IRemoteBounceFactory {
        ITask CreateRemoteBounce(Task<string> bounceArguments, Task<string> workingDirectory, Task<string> machine);
    }

    public interface IRemoteBounceFactory<T> {
        ITask CreateRemoteBounce(Task<string> bounceArguments, T machine);
    }
}