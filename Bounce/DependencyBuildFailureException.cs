namespace Bounce.Framework {
    internal class DependencyBuildFailureException : TargetException {
        public DependencyBuildFailureException(ITarget target, string message) : base(target, message) {
        }
    }
}