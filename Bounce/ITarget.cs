using System;
using System.Collections.Generic;

namespace Bounce.Framework {
    public interface ITarget {
        IEnumerable<ITarget> Dependencies { get; }
        DateTime? LastBuilt { get; }
        void Build();
        void Clean();
    }
}