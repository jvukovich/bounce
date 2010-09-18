using System.Collections.Generic;
using System.Linq;

namespace Bounce.Framework {
    public class TargetBuilder {
        public TaskWalker Walker;
        public HashSet<ITask> BuiltTasks;

        public TargetBuilder() : this(new TaskWalker()) {
            BuiltTasks = new HashSet<ITask>();
        }

        public TargetBuilder(TaskWalker walker) {
            Walker = walker;
        }

        public void Build(ITask task) {
            Walker.Walk(task, t => t.BeforeBuild(), BuildIfNotAlreadyBuilt);
        }

        public void BuildIfNotAlreadyBuilt(ITask task) {
            if (!BuiltTasks.Contains(task)) {
                task.Build();
                BuiltTasks.Add(task);
            }
        }

        public void Clean(ITask task) {
            Walker.Walk(task, b => b.Clean(), null);
        }
    }
}