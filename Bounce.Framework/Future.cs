using System.Collections.Generic;
using System.IO;

namespace Bounce.Framework {
    public abstract class Future<T> : ITask {
        public abstract T Value { get; }
        public abstract IEnumerable<ITask> Dependencies { get; }
        public virtual void Build(IBounce bounce) {}
        public virtual void Clean(IBounce bounce) {}

        public static implicit operator Future<T>(T v) {
            return new PlainValue<T>(v);
        }

        public virtual bool IsLogged { get { return false; } }

        public virtual void Describe(TextWriter output) { }
    }
}