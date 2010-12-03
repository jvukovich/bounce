using System.Collections.Generic;
using System.IO;

namespace Bounce.Framework {
    public interface ITask {
        IEnumerable<ITask> Dependencies { get; }
        void Build(IBounce bounce);
        void Clean(IBounce bounce);
        bool IsLogged { get; }
        void Describe(TextWriter output);
    }
}