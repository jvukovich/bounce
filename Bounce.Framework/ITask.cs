using System.Collections.Generic;

namespace Bounce.Framework {
    public interface ITask {
        IEnumerable<ITask> Dependencies { get; }
        void Build(IBounce bounce);
        void Clean(IBounce bounce);
        bool IsLogged { get; }
    }
}