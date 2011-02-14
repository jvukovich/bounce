using System.IO;

namespace Bounce.Framework.Tests.Features {
    class PrintTask : Task {
        [Dependency]
        public Task<string> Description;

        private readonly TextWriter Output;

        public PrintTask(TextWriter output) {
            Output = output;
        }

        public override void Build() {
            Output.WriteLine(Description.Value);
        }
    }
}