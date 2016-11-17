using System;

namespace Bounce.Console {
    internal class Program {
        private static int Main(string[] args) {
            var exitCode = new BounceAssemblyRunner().Run(args);
            return exitCode;
        }
    }
}