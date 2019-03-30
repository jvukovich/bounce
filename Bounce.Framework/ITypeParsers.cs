using System;

namespace Bounce.Framework
{
    public interface ITypeParsers
    {
        T Parse<T>(string parameterValue);
        object Parse(Type type, string parameterValue);
        string Generate<T>(T parameterValue);

        void RegisterTypeParser<T>(ITypeParser parser);
        ITypeParser TypeParser(Type type);
    }
}