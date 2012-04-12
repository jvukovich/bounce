using Bounce.Framework.Obsolete;

namespace Bounce.Framework {
    public class NoTasksException : BounceException {
        public NoTasksException() : base("[Task] method not found") {
        }
    }
}