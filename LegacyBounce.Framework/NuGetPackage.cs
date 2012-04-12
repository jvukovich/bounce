using System.Text.RegularExpressions;

namespace LegacyBounce.Framework
{
    public class NuGetPackage : NuGetTask {
        [Dependency] public Task<string> Spec;
        private string _packagePath;

        public override void Build(IBounce bounce)
        {
            var output = bounce.ShellCommand.ExecuteAndExpectSuccess(NuGetExePath.Value, string.Format(@"pack ""{0}""", Spec.Value));

            var packageRegex = new Regex(@"Successfully created package '(.*)'.");

            var match = packageRegex.Match(output.Output);
            if (match.Success)
            {
                _packagePath = match.Groups[1].Value;
            } else
            {
                throw new TaskException(this, "could not parse package name");
            }
        }

        public Task<string> Package
        {
            get { return this.WhenBuilt(() => _packagePath); }
        }
    }
}