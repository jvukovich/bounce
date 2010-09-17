using System.Collections.Generic;

namespace Bounce.Framework {
    public interface IParameter {
        void Parse(string parameterValue, ITypeParsers typeParsers);
        string Name { get; }
        bool Required { get; }
        bool HasValue { get; }
        IEnumerable<object> AvailableValues { get; }
        object DefaultValue { get; }
    }

    public interface IParameter<T> : IValue<T>, IParameter {
    }
}