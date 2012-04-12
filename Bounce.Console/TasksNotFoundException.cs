using System;
using System.IO;
using System.Runtime.Serialization;

namespace Bounce.Console {
    [Serializable]
    internal class TasksNotFoundException : BounceConsoleException {
        protected TasksNotFoundException(SerializationInfo info,
                        StreamingContext context) {}

        public TasksNotFoundException() {
        }

        public override void Explain(TextWriter writer)
        {
            writer.WriteLine("assembly contains no [Task] methods. Try adding something like this:");
            writer.WriteLine();
            writer.WriteLine(
                @"public class ATaskClass {
    [Bounce.Framework.Task]
    public void MyTask () {
        // do something
    }
}
");
        }
    }
}