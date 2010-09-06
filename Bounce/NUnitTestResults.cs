using System;
using System.Collections.Generic;
using System.Linq;

namespace Bounce {
    public class NUnitTestResults : ITarget {
        public IValue<IEnumerable<string>> DllPaths;

        public IEnumerable<ITarget> Dependencies {
            get { return new[] {DllPaths}; }
        }

        public DateTime? LastBuilt {
            get { return null; }
        }

        public void Build() {
            IEnumerable<string> testDlls = DllPaths.Value.Where(dll => dll.EndsWith("Tests.dll"));
            foreach (var dllPath in testDlls) {
                Console.WriteLine("running unit tests for: " + dllPath);
            }
        }

        public void Clean() {
        }
    }
}