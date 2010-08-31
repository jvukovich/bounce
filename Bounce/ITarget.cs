using System;
using System.Collections.Generic;

namespace Bounce {
    public interface ITarget {
        IEnumerable<ITarget> Dependencies { get; }
        DateTime? LastBuilt { get; }
        void Build();
        void Clean();
    }
}