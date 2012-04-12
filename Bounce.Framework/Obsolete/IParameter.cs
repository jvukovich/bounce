using System.Collections.Generic;

namespace Bounce.Framework.Obsolete {
    public interface IParameter {
        void Parse(string parameterValue, ITypeParsers typeParsers);
        string Generate(ITypeParsers typeParsers);
        string Name { get; }
        bool Required { get; }
        bool HasValue { get; }
        IEnumerable<object> AvailableValues { get; }
        object DefaultValue { get; }
    }

    public interface IParameter<T> : IParameter
    {
        T Value { get; }
        Parameter<T> WithValue(T value);
    }
}