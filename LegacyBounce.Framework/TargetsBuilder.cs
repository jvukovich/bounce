using System.Collections.Generic;

namespace LegacyBounce.Framework {
    public class TargetsBuilder : ITargetsBuilder {

        public void BuildTargets(ITargetBuilderBounce bounce, IEnumerable<Target> targets, IBounceCommand command) {
            foreach (var target in targets) {
                if (target.Task != null) {
                    InvokeTask(bounce, command, target.Name, target.Task);
                }
            }
        }

        private static void InvokeTask(ITargetBuilderBounce bounce, IBounceCommand command, string name, IObsoleteTask task) {
            using (var targetScope = bounce.TaskScope(task, command, name)) {
                bounce.Invoke(command, task);
                targetScope.TaskSucceeded();
            }
        }
    }
}
