using System;
using System.Collections.Generic;

namespace LegacyBounce.Framework {
    public class All<T> : All {
        public Task<T> Result;

        public All(Func<T> result, params IObsoleteTask [] tasks) : base(tasks) {
            Result = new DependentFuture<T>(this, result);
        }
    }

    public class All : Task {
        [Dependency]
        public IEnumerable<IObsoleteTask> Tasks;

        public All(params IObsoleteTask[] tasks) {
            Tasks = tasks;
        }

        public override bool IsLogged { get { return false; } }

        public override bool IsPure
        {
            get
            {
                return true;
            }
        }
    }
}
