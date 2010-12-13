using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Bounce.Framework {
    public abstract class Future<T> : ITask {
        public abstract T Value { get; }
        public abstract IEnumerable<ITask> Dependencies { get; }
        public virtual void Invoke(BounceCommand command, IBounce bounce) { }

        public static implicit operator Future<T>(T v) {
            return new PlainValue<T>(v);
        }

        public virtual bool IsLogged { get { return false; } }

        public virtual void Describe(TextWriter output) { }
    }

    public abstract class TaskWithValue<T> : Future<T> {
        public override IEnumerable<ITask> Dependencies {
            get {
                return TaskDependencyFinder.Instance.GetDependenciesFor(this).Concat(RegisterAdditionalDependencies());
            }
        }

        protected virtual IEnumerable<ITask> RegisterAdditionalDependencies() {
            return new ITask[0];
        }
    }
}