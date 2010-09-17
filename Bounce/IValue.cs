namespace Bounce.Framework {
    public interface IValue<T> : ITask {
        T Value { get; }
    }
}