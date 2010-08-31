using System;
using System.Collections.Generic;

namespace Bounce {
    class TypeParsers : Dictionary<Type, Func<string, object>>, ITypeParsers {
        public T Parse<T>(string parameterValue) {
            Func<string, object> parser = this[typeof(T)];
            return (T) parser(parameterValue);
        }
    }
}