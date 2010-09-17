namespace Bounce.Framework {
    public interface IIisWebSiteDirectory : ITarget {
        IValue<string> Path { get; }
    }
}