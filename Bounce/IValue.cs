namespace Bounce.Framework {
    public interface IValue<T> : ITarget {
        T Value { get; }
    }
}