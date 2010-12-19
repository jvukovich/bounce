using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Bounce.Framework {
    public abstract class Future<T> : ITask {
        public T Value {
            get {
                if (Invoked) {
                    return GetValue();
                } else {
                    throw new DependencyBuildFailureException(this, "no value for future");
                }
            }
        }

        private bool Invoked = true;

        public abstract T GetValue();
        public abstract IEnumerable<ITask> Dependencies { get; }

        public void Invoke(BounceCommand command, IBounce bounce) {
            InvokeFuture(command, bounce);
            Invoked = true;
        }

        public virtual void InvokeFuture(BounceCommand command, IBounce bounce) {}

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