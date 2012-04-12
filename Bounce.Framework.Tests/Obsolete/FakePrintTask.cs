using System.IO;
using Bounce.Framework.Obsolete;

namespace Bounce.Framework.Tests.Obsolete {
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