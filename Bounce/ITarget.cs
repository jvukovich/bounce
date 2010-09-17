using System.Collections.Generic;

namespace Bounce.Framework {
    public interface ITarget {
        IEnumerable<ITarget> Dependencies { get; }
        void BeforeBuild();
        void Build();
        void Clean();
    }
}