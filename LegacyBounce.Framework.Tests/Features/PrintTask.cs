using System;
using System.IO;

namespace LegacyBounce.Framework.Tests.Features {
    public class PrintTask : Task {
        [Dependency]
        public Task<string> Description;

        private readonly TextWriter Output;

        public PrintTask() : this(Console.Out) {
        }

        public PrintTask(TextWriter output) {
            Output = output;
        }

        public override void Build() {
            Output.WriteLine(Description.Value);
        }
    }
}