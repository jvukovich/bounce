using System;
using System.Collections.Generic;

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
            foreach (var dllPath in DllPaths.Value) {
                Console.WriteLine("running unit tests for: " + dllPath);
            }
        }

        public void Clean() {
        }
    }
}