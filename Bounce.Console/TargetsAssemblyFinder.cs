using System;
using System.IO;
using System.Linq;

namespace Bounce.Console {
    public class BounceDirectoryExecutable {
        public string Executable;
        public BounceDirectoryExecutableType ExecutableType;
    }

    public enum BounceDirectoryExecutableType {
        Targets,
        BeforeBounce
    }

    class TargetsAssemblyFinder : ITargetsAssemblyFinder {
        public static string TargetsDllPath = Path.Combine("Bounce", "Targets.dll");

        public BounceDirectoryExecutable FindTargetsAssembly() {
            return FindTargetsAssembly(Directory.GetCurrentDirectory());
        }

        public BounceDirectoryExecutable FindTargetsAssembly(string currentDir) {
            if (String.IsNullOrEmpty(currentDir)) {
                return null;
            }

            var bounceDirectoryExecutable = BounceDirectoryExists(currentDir);
            if (bounceDirectoryExecutable != null) {
                return bounceDirectoryExecutable;
            } else {
                return FindTargetsAssembly(Path.GetDirectoryName(currentDir));
            }
        }

        private BounceDirectoryExecutable BounceDirectoryExists(string currentDir) {
            var targetsDll = Path.Combine(currentDir, TargetsDllPath);
            if (File.Exists(targetsDll)) {
                return new BounceDirectoryExecutable {
                    Executable = targetsDll,
                    ExecutableType = BounceDirectoryExecutableType.Targets
                };
            }

            var beforeBounceScript = Directory.GetFiles(Path.Combine(currentDir, @"Bounce"), "beforebounce.*").FirstOrDefault();
            if (beforeBounceScript != null) {
                return new BounceDirectoryExecutable {
                    Executable = beforeBounceScript,
                    ExecutableType = BounceDirectoryExecutableType.BeforeBounce
                };
            } else {
                return null;
            }
        }
    }
}