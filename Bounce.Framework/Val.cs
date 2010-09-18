using System.Collections.Generic;

namespace Bounce.Framework {
    public abstract class Val<T> : ITask {
        public abstract T Value { get; }
        public abstract IEnumerable<ITask> Dependencies { get; }
        public abstract void BeforeBuild();
        public abstract void Build();
        public abstract void Clean();

        public static implicit operator Val<T>(T v) {
            return new PlainValue<T>(v);
        }
    }
}