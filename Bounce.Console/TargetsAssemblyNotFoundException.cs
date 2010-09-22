using System.IO;

namespace Bounce.Console {
    public class TargetsAssemblyNotFoundException : BounceConsoleException {
        public override void Explain(TextWriter writer) {
            writer.WriteLine(@"unable to find Bounce\Targets.dll assembly in this or any parent directory");
        }
    }
}