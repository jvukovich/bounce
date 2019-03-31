using System;
using System.Collections.Generic;
using System.Linq;

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

    internal class TypeParsers : Dictionary<Type, ITypeParser>, ITypeParsers
    {
        private static TypeParsers _default;

        public static ITypeParsers Default => _default ?? (_default = CreateWithStandardTypeParsers());

        private static TypeParsers CreateWithStandardTypeParsers()
        {
            var typeParsers = new TypeParsers();

            typeParsers.RegisterTypeParser<int>(new IntParser());
            typeParsers.RegisterTypeParser<string>(new StringParser());
            typeParsers.RegisterTypeParser<DateTime>(new DateTimeParser());
            typeParsers.RegisterTypeParser<bool>(new BooleanParser());

            return typeParsers;
        }

        public string Generate<T>(T parameterValue)
        {
            return this[typeof(T)].Generate(parameterValue);
        }

        public void RegisterTypeParser<T>(ITypeParser parser)
        {
            Add(typeof(T), parser);
        }

        public ITypeParser TypeParser(Type type)
        {
            if (type.IsEnum)
                return new EnumParser(type);

            return ContainsKey(type) ? this[type] : new NullTypeParser(type);
        }

        public T Parse<T>(string parameterValue)
        {
            return (T) Parse(typeof(T), parameterValue);
        }

        public object Parse(Type type, string parameterValue)
        {
            var parser = TypeParser(type);

            if (parser != null)
                return parser.Parse(parameterValue);

            TypeParserNotFound(parameterValue, type);

            return null;
        }

        public static void TypeParserNotFound(string parameterValue, Type type)
        {
            throw new Exception($"Could not parse '{parameterValue}' for type '{type}'.");
        }
    }

    internal class NullTypeParser : ITypeParser
    {
        private readonly Type _type;

        public NullTypeParser(Type type)
        {
            _type = type;
        }

        public object Parse(string s)
        {
            TypeParsers.TypeParserNotFound(s, _type);
            return null;
        }

        public string Generate(object o)
        {
            return null;
        }

        public string Description => _type.Name;
    }

    public class EnumParser : ITypeParser
    {
        private readonly Type _enumType;

        public EnumParser(Type enumType)
        {
            _enumType = enumType;
        }

        public object Parse(string s)
        {
            return Enum.Parse(_enumType, s, true);
        }

        public string Generate(object o)
        {
            return null;
        }

        public string Description
        {
            get
            {
                var values = Enum.GetValues(_enumType).Cast<object>();
                return "{" + string.Join(",", values.Select(v => v.ToString()).ToArray()) + "}";
            }
        }
    }

    public interface ITypeParser
    {
        object Parse(string s);
        string Generate(object o);
        string Description { get; }
    }

    internal class IntParser : ITypeParser
    {
        public object Parse(string s)
        {
            return int.Parse(s);
        }

        public string Generate(object o)
        {
            return ((int) o).ToString();
        }

        public string Description => "int";
    }

    internal class BooleanParser : ITypeParser
    {
        public object Parse(string s)
        {
            return bool.Parse(s);
        }

        public string Generate(object o)
        {
            return ((bool) o).ToString().ToLower();
        }

        public string Description => "bool";
    }

    internal class StringParser : ITypeParser
    {
        public object Parse(string s)
        {
            return s;
        }

        public string Generate(object s)
        {
            return s.ToString();
        }

        public string Description => "string";
    }

    internal class DateTimeParser : ITypeParser
    {
        public object Parse(string s)
        {
            return DateTime.Parse(s);
        }

        public string Generate(object s)
        {
            return ((DateTime) s).ToString("yyyy-MM-dd H:mm:ss");
        }

        public string Description => "datetime";
    }
}