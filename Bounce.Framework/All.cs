using System;
using System.Collections.Generic;

namespace Bounce.Framework {
    public class All<T> : All {
        public Val<T> Result;

        public All(Func<T> result, params ITask [] tasks) : base(tasks) {
            Result = new FutureValue<T>(this, result);
        }
    }

    public class All : Task {
        [Dependency]
        public IEnumerable<ITask> Tasks;

        public All(params ITask[] tasks) {
            Tasks = tasks;
        }
    }
}
