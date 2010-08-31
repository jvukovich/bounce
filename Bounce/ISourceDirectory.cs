namespace Bounce {
    public interface ISourceDirectory : ITarget {
        IValue<string> Path { get; }
    }
}