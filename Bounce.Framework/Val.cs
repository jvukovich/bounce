using System.Collections.Generic;

namespace Bounce.Framework {
    public abstract class Val<T> : ITask {
        public abstract T Value { get; }
        public abstract IEnumerable<ITask> Dependencies { get; }
        public void BeforeBuild(IBounce bounce) {}
        public void Build(IBounce bounce) {}
        public void Clean(IBounce bounce) {}

        public static implicit operator Val<T>(T v) {
            return new PlainValue<T>(v);
        }
    }
}