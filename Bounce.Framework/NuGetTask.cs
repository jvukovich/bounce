namespace Bounce.Framework
{
    public class NuGetTask : Task
    {
        [Dependency] public Task<string> NuGetExePath;
    }
}