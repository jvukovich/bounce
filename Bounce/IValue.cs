using System;
using System.Collections.Generic;

namespace Bounce {
    public interface IValue<T> : ITarget {
        T Value { get; }
    }
}