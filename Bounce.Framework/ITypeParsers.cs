using System;

namespace Bounce.Framework {
    public interface ITypeParsers {
        T Parse<T>(string parameterValue);
        string Generate<T>(T parameterValue);

        void RegisterTypeParser<T>(ITypeParser parser);
    }
}