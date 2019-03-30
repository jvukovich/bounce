using System.IO;

namespace Bounce.Console
{
    internal class TargetsAttributeNotFoundException : BounceConsoleException
    {
        public override void Explain(TextWriter writer)
        {
            writer.WriteLine("assembly contains no [Targets] method. Try adding something like this:");
            writer.WriteLine();
            writer.WriteLine(
                @"public class BuildTargets {
    [Bounce.Framework.Obsolete.Targets]
    public static object Targets (IParameters parameters) {
        return new {
            MyTarget = ...
        };
    }
}
");
        }
    }
}