using System;

namespace Bounce.Framework.Obsolete {
    public class ShellExecutableNotFoundException : BounceException {
        public ShellExecutableNotFoundException(string pathToExecutable) : base(String.Format("could not find path for executable: `{0}'", pathToExecutable)) {}

        public override void Explain(System.IO.TextWriter stderr) {
            stderr.WriteLine(Message);
        }
    }
}