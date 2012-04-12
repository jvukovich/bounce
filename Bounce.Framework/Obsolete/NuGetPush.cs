namespace Bounce.Framework.Obsolete
{
    public class NuGetPush : NuGetTask {
        [Dependency] public Task<string> Package;
        [Dependency] public Task<string> Source;
        [Dependency] public Task<string> ApiKey;

        public NuGetPush()
        {
            Source = "http://packages.nuget.org/v1/";
        }

        public override void Build(IBounce bounce)
        {
            bounce.ShellCommand.ExecuteAndExpectSuccess(NuGetExePath.Value, string.Format(@"push ""{0}"" {1} -source {2}" , Package.Value, ApiKey.Value, Source.Value));
        }
    }
}