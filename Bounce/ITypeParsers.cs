namespace Bounce {
    public interface ITypeParsers {
        T Parse<T>(string parameterValue);
    }
}