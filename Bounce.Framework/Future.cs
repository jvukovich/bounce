using System.Collections.Generic;
using System.IO;

namespace Bounce.Framework {
    public abstract class Future<T> : ITask {
        public abstract T Value { get; }
        public abstract IEnumerable<ITask> Dependencies { get; }
        public void Build(IBounce bounce) {}
        public void Clean(IBounce bounce) {}

        public static implicit operator Future<T>(T v) {
            return new PlainValue<T>(v);
        }

        public bool IsLogged { get { return false; }}

        public void Describe(TextWriter output) { }
    }
}