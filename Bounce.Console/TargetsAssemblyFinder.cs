using System;
using System.IO;

namespace Bounce.Console {
    class TargetsAssemblyFinder : ITargetsAssemblyFinder {
        public static string TargetsDllPath = Path.Combine("Bounce", "Targets.dll");

        public string FindTargetsAssembly() {
            return FindTargetsAssembly(Directory.GetCurrentDirectory());
        }

        public string FindTargetsAssembly(string currentDir) {
            if (String.IsNullOrEmpty(currentDir)) {
                return null;
            }

            var targetsDll = Path.Combine(currentDir, TargetsDllPath);
            if (File.Exists(targetsDll)) {
                return targetsDll;
            } else {
                return FindTargetsAssembly(Path.GetDirectoryName(currentDir));
            }
        }
    }
}