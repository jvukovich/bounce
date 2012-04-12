namespace Bounce.Framework.Obsolete {
    internal class DependencyBuildFailureException : TaskException {
        public DependencyBuildFailureException(IObsoleteTask task, string message) : base(task, message) {
        }
    }
}