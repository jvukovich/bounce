using System.Collections.Generic;
using System.IO;

namespace Bounce.Framework {
    public interface ITask {
        string Name { get; }
        string FullName { get; }
        IEnumerable<string> AllNames { get; }
        IEnumerable<ITaskParameter> Parameters { get; }
        void Invoke(Arguments arguments);
    }
}