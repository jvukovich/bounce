namespace LegacyBounce.Framework {
    public interface IRemoteBounceFactory {
        IObsoleteTask CreateRemoteBounce(Task<string> bounceArguments, Task<string> workingDirectory, Task<string> machine);
    }

    public interface IRemoteBounceFactory<T> {
        IObsoleteTask CreateRemoteBounce(Task<string> bounceArguments, T machine);
    }
}