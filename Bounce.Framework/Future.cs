using System;
using System.Collections.Generic;
using System.IO;

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
            return new ImmediateValue<T>(v);
        }

        public virtual bool IsLogged { get { return false; } }

        public virtual void Describe(TextWriter output) { }
    }
}