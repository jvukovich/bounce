using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Bounce.Framework {
    public abstract class Task : ITask {
        public virtual IEnumerable<TaskDependency> Dependencies {
            get {
                return TaskDependencyFinder.Instance.GetDependenciesFor(this).Concat(RegisterAdditionalDependencies());
            }
        }

        public virtual void Invoke(BounceCommand command, IBounce bounce)
        {
            command.InvokeCommand(() => Build(bounce), () => Clean(bounce));
        }

        public virtual void Build(IBounce bounce) {
            Build();
        }

        public virtual void Build() {}

        public virtual void Clean(IBounce bounce) {
            Clean();
        }

        public virtual void Clean() {}

        public virtual bool IsLogged { get { return true; } }

        public virtual void Describe(TextWriter output)
        {
            output.WriteLine("task: {0}", GetType().Name);

            foreach(TaskDependency dependency in TaskDependencyFinder.Instance.GetDependenciesFor(this))
            {
                output.Write("    {0}:", dependency.Name);
                dependency.Task.Describe(output);
                output.WriteLine();
            }
        }

        protected virtual IEnumerable<TaskDependency> RegisterAdditionalDependencies() {
            return new TaskDependency[0];
        }
    }
}