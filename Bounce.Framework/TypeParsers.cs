using System;
using System.Collections.Generic;

namespace Bounce.Framework {
    class TypeParsers : Dictionary<Type, Func<string, object>>, ITypeParsers {
        public static TypeParsers CreateWithStandardTypeParsers() {
            var typeParsers = new TypeParsers();
            typeParsers.RegisterTypeParser(s => int.Parse(s));
            typeParsers.RegisterTypeParser(s => s);
            typeParsers.RegisterTypeParser(s => DateTime.Parse(s));
            return typeParsers;
        }

        public void RegisterTypeParser<T>(Func<string, T> parser) {
            Add(typeof(T), s => parser(s));
        }

        public T Parse<T>(string parameterValue) {
            Func<string, object> parser = this[typeof(T)];
            return (T) parser(parameterValue);
        }
    }
}