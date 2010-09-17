namespace Bounce.Framework {
    public interface ITypeParsers {
        T Parse<T>(string parameterValue);
    }
}