using System.Collections.Generic;
using System.Linq;

namespace Bounce.Framework {
    public static class TaskExtensions
    {
        public static IEnumerable<ITask> GetNonNullDependencies(this ITask task) {
            var deps = task.Dependencies;
            if (deps == null) {
                return new ITask[0];
            } else {
                return deps.Where(d => d != null);
            }
        }
    }
}