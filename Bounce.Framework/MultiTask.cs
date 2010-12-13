using System.Collections.Generic;

namespace Bounce.Framework
{
    public abstract class MultiTask : Task
    {
        public override void Invoke(BounceCommand command, IBounce bounce) {
            foreach (var task in GetTasks(bounce))
            {
                bounce.Invoke(command, task);
            }
        }

        public abstract IEnumerable<ITask> GetTasks(IBounce bounce);
    }
}