using System.Collections.Generic;

namespace Bounce.Framework {
    public interface ITarget {
        IEnumerable<ITarget> Dependencies { get; }
        void Build();
        void Clean();
    }
}