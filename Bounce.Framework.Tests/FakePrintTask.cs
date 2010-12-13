using System.IO;

namespace Bounce.Framework.Tests {
    class FakePrintTask : Task
    {
        private StringWriter Output;
        private readonly string Name;

        public FakePrintTask(StringWriter output, string name)
        {
            Output = output;
            Name = name;
        }

        public override void Build(IBounce bounce) {
            Output.Write(Name + ";");
        }
    }
}