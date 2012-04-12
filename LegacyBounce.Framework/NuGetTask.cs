namespace LegacyBounce.Framework
{
    public class NuGetTask : Task
    {
        [Dependency] public Task<string> NuGetExePath;
    }
}