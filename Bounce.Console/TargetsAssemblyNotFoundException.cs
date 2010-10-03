using System;
using System.IO;

namespace Bounce.Console {
    public class TargetsAssemblyNotFoundException : BounceConsoleException {
        public override void Explain(TextWriter writer) {
            writer.WriteLine(String.Format(@"unable to find {0} assembly in this or any parent directory", TargetsAssemblyFinder.TargetsDllPath));
        }
    }
}