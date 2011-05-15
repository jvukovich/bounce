using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Bounce.Framework {
    public abstract class Task : ITask, ICommandableTask {
        protected Task()
        {
            DependsOn = new ITask[0];
        }

        public virtual IEnumerable<TaskDependency> Dependencies {
            get {
                return TaskDependencyFinder.Instance.GetDependenciesFor(this)
                    .Concat(RegisterAdditionalDependencies())
                    .Concat(DependsOn.Select(t => new TaskDependency(t)));
            }
        }

        public virtual void Invoke(IBounceCommand command, IBounce bounce)
        {
            command.InvokeCommand(() => Build(bounce), () => Clean(bounce), () => Describe(bounce));
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

        public virtual bool IsPure { get { return false; } }

        public virtual void Describe(IBounce bounce) {
            Describe(bounce.LogOptions.StdOut);
        }

        public virtual void Describe(TextWriter output)
        {
            output.WriteLine("task: {0}", GetType().Name);

            foreach(TaskDependency dependency in TaskDependencyFinder.Instance.GetDependenciesFor(this))
            {
                output.Write("    {0}: ", dependency.Name);
                dependency.Task.Describe(output);
                output.WriteLine(dependency.Task.SmallDescription);
            }
        }

        public string SmallDescription {
            get { return GetType().Name; }
        }

        protected virtual IEnumerable<TaskDependency> RegisterAdditionalDependencies() {
            return new TaskDependency[0];
        }

        public IEnumerable<ITask> DependsOn { get; set; }
    }

    public interface ICommandableTask {
        void Build(IBounce bounce);
        void Clean(IBounce bounce);
    }
}