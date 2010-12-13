using System;
using System.Collections.Generic;

namespace Bounce.Framework {
    public class OptionalTask<T> : EnumerableFuture<T> where T : ITask {
        private readonly Future<bool> Condition;
        private readonly Func<T> GetResult;

        public OptionalTask(Future<bool> condition, Func<T> getResult) {
            Condition = condition;
            GetResult = getResult;
        }

        public override IEnumerable<T> GetTasks(IBounce bounce) {
            if (Condition.Value) {
                return new[] {GetResult()};
            } else {
                return new T[0];
            }
        }
    }
}