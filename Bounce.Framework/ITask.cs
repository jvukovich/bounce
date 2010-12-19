using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Bounce.Framework {
    public interface ITask {
        IEnumerable<TaskDependency> Dependencies { get; }
        void Invoke(BounceCommand command, IBounce bounce);
        bool IsLogged { get; }
        void Describe(TextWriter output);
    }
}