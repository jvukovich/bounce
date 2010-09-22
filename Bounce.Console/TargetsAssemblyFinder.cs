using System;
using System.IO;

namespace Bounce.Console {
    class TargetsAssemblyFinder : ITargetsAssemblyFinder {
        public string FindTargetsAssembly() {
            return FindTargetsAssembly(Directory.GetCurrentDirectory());
        }

        public string FindTargetsAssembly(string currentDir) {
            if (String.IsNullOrEmpty(currentDir)) {
                return null;
            }

            var targetsDll = Path.Combine(Path.Combine(currentDir, "Bounce"), "Targets.dll");
            if (File.Exists(targetsDll)) {
                return targetsDll;
            } else {
                return FindTargetsAssembly(Path.GetDirectoryName(currentDir));
            }
        }
    }
}