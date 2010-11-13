using System;

namespace Bounce.Framework {
    public interface ITypeParsers {
        T Parse<T>(string parameterValue);
        void RegisterTypeParser<T>(Func<string, T> parser);
    }
}