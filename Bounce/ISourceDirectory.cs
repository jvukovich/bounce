namespace Bounce.Framework {
    public interface ISourceDirectory : ITarget {
        IValue<string> Path { get; }
    }
}