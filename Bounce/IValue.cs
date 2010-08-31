namespace Bounce {
    public interface IValue<T> : ITarget {
        T Value { get; }
    }
}