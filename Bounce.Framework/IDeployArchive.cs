namespace Bounce.Framework {
    public interface IDeployArchive {
        Task<string> Add(Task<string> from, Task<string> archivePath);
    }
}