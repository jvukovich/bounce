namespace Bounce.Framework.Obsolete {
    public interface IDeployArchive {
        Task<string> Add(Task<string> from, Task<string> archivePath);
    }
}