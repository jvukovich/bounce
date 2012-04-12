namespace Bounce.Framework.Obsolete
{
    public class NuGetTask : Task
    {
        [Dependency] public Task<string> NuGetExePath;
    }
}