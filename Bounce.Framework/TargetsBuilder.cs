using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bounce.Framework {
    public class TargetsBuilder : ITargetsBuilder {

        public void BuildTargets(ITargetBuilderBounce bounce, IEnumerable<Target> targets, IBounceCommand command) {
            foreach (var target in targets) {
                using (var targetScope = bounce.TaskScope(target.Task, command, target.Name)) {
                    bounce.Invoke(command, target.Task);
                    targetScope.TaskSucceeded();
                }
            }
        }
    }
}
