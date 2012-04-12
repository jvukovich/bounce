using System.Collections.Generic;
using System.IO;

namespace LegacyBounce.Framework {
    public interface IObsoleteTask {
        IEnumerable<TaskDependency> Dependencies { get; }
        void Invoke(IBounceCommand command, IBounce bounce);
        bool IsLogged { get; }
        void Describe(TextWriter output);
        string SmallDescription { get; }
        bool IsPure { get; }
    }
}