namespace Bounce.Framework {
    internal class TargetException : BounceException {
        public ITarget Target { get; private set; }

        public TargetException(ITarget target, string message) : base(message) {
            Target = target;
        }
    }
}