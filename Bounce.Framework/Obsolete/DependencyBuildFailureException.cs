namespace Bounce.Framework.Obsolete {
    internal class DependencyBuildFailureException : TaskException {
        public DependencyBuildFailureException(ITask task, string message) : base(task, message) {
        }
    }
}