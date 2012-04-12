using System;
using System.Collections.Generic;
using Bounce.Framework.Obsolete;

namespace Bounce.Framework {
    class TypeParsers : Dictionary<Type, ITypeParser>, ITypeParsers {
        private static TypeParsers _default;

        public static ITypeParsers Default {
            get {
                if (_default == null) {
                    _default = CreateWithStandardTypeParsers();
                }
                return _default;
            }
        }

        private static TypeParsers CreateWithStandardTypeParsers() {
            var typeParsers = new TypeParsers();
            typeParsers.RegisterTypeParser<int>(new IntParser());
            typeParsers.RegisterTypeParser<string>(new StringParser());
            typeParsers.RegisterTypeParser<DateTime>(new DateTimeParser());
            typeParsers.RegisterTypeParser<bool>(new BooleanParser());
            return typeParsers;
        }

        public string Generate<T>(T parameterValue) {
            ITypeParser parser = this[typeof(T)];
            return parser.Generate(parameterValue);
        }

        public void RegisterTypeParser<T>(ITypeParser parser) {
            Add(typeof(T), parser);
        }

        public T Parse<T>(string parameterValue) {
            ITypeParser parser = this[typeof(T)];
            return (T) parser.Parse(parameterValue);
        }

        public object Parse(Type type, string parameterValue) {
            if (!ContainsKey(type)) {
                throw new TypeParserNotFoundException(parameterValue, type);
            }

            ITypeParser parser = this[type];
            return parser.Parse(parameterValue);
        }
    }

    public class TypeParserNotFoundException : Exception {
        public TypeParserNotFoundException(string value, Type type) : base(string.Format("could not parse `{0}' for type `{1}'", value, type)) {}
    }

    public interface ITypeParser {
        object Parse(string s);
        string Generate(object o);
    }

    class IntParser : ITypeParser {
        public object Parse(string s) {
            return int.Parse(s);
        }

        public string Generate(object o) {
            return ((int) o).ToString();
        }
    }

    class BooleanParser : ITypeParser {
        public object Parse(string s) {
            return bool.Parse(s);
        }

        public string Generate(object o) {
            return ((bool) o).ToString().ToLower();
        }
    }

    class StringParser : ITypeParser {
        public object Parse(string s) {
            return s;
        }

        public string Generate(object s) {
            return s.ToString();
        }
    }

    class DateTimeParser : ITypeParser {
        public object Parse(string s) {
            return DateTime.Parse(s);
        }

        public string Generate(object s) {
            return ((DateTime) s).ToString("yyyy-MM-dd H:mm:ss");
        }
    }
}