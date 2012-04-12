using System;
using System.Collections.Generic;
using System.Linq;
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

        public ITypeParser TypeParser(Type type) {
            if (type.IsEnum) {
                return new EnumParser(type);
            } else if (ContainsKey(type)) {
                return this[type];
            } else {
                return null;
            }
        }

        public T Parse<T>(string parameterValue) {
            return (T) Parse(typeof (T), parameterValue);
        }

        public object Parse(Type type, string parameterValue) {
            ITypeParser parser = TypeParser(type);
            if (parser != null) {
                return parser.Parse(parameterValue);
            } else {
                throw new TypeParserNotFoundException(parameterValue, type);
            }
        }
    }

    public class EnumParser : ITypeParser {
        private readonly Type _enumType;

        public EnumParser(Type enumType) {
            _enumType = enumType;
        }

        public object Parse(string s) {
            return Enum.Parse(_enumType, s, true);
        }

        public string Generate(object o) {
            throw new NotImplementedException();
        }

        public string Description {
            get {
                var values = Enum.GetValues(_enumType).Cast<object>();
                return "{" + String.Join(",", values.Select(v => v.ToString()).ToArray()) + "}";
            }
        }
    }

    public class TypeParserNotFoundException : Exception {
        public TypeParserNotFoundException(string value, Type type) : base(string.Format("could not parse `{0}' for type `{1}'", value, type)) {}
    }

    public interface ITypeParser {
        object Parse(string s);
        string Generate(object o);
        string Description { get; }
    }

    class IntParser : ITypeParser {
        public object Parse(string s) {
            return int.Parse(s);
        }

        public string Generate(object o) {
            return ((int) o).ToString();
        }

        public string Description {
            get { return "int"; }
        }
    }

    class BooleanParser : ITypeParser {
        public object Parse(string s) {
            return bool.Parse(s);
        }

        public string Generate(object o) {
            return ((bool) o).ToString().ToLower();
        }

        public string Description {
            get { return "bool"; }
        }
    }

    class StringParser : ITypeParser {
        public object Parse(string s) {
            return s;
        }

        public string Generate(object s) {
            return s.ToString();
        }

        public string Description {
            get { return "string"; }
        }
    }

    class DateTimeParser : ITypeParser {
        public object Parse(string s) {
            return DateTime.Parse(s);
        }

        public string Generate(object s) {
            return ((DateTime) s).ToString("yyyy-MM-dd H:mm:ss");
        }

        public string Description {
            get { return "datetime"; }
        }
    }
}