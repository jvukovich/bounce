using System.Collections.Generic;

namespace Bounce.Framework {
    public interface ITask {
        IEnumerable<ITask> Dependencies { get; }
        void BeforeBuild();
        void Build();
        void Clean();
    }
}