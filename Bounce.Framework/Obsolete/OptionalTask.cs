using System;
using System.Collections.Generic;

namespace Bounce.Framework.Obsolete {
    public class OptionalTask<T> : EnumerableFuture<T> where T : IObsoleteTask {
        [Dependency]
        private readonly Task<bool> Condition;
        private readonly Func<T> GetResult;
        private readonly bool Invert;

        public OptionalTask(Task<bool> condition, Func<T> getResult, bool invert) {
            Condition = condition;
            GetResult = getResult;
            Invert = invert;
        }

        public override IEnumerable<T> GetTasks(IBounce bounce) {
            if ((Condition.Value ^ Invert)) {
                return new[] {GetResult()};
            } else {
                return new T[0];
            }
        }
    }
}