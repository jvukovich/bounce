using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Bounce.Framework {
    public abstract class Task : ITask {
        public virtual IEnumerable<ITask> Dependencies {
            get {
                return TaskDependencyFinder.Instance.GetDependenciesFor(this).Concat(RegisterAdditionalDependencies());
            }
        }

        public virtual void Build(IBounce bounce) {
            Build();
        }

        public virtual void Build() {}

        public virtual void Clean(IBounce bounce) {
            Clean();
        }

        public virtual void Clean() {}

        public virtual bool IsLogged { get { return true; } }

        public virtual void Describe(TextWriter output)
        {
            output.WriteLine("task: {0}", GetType().Name);

            foreach(KeyValuePair<string, ITask> dependency in TaskDependencyFinder.Instance.GetDependencyFieldsFor(this))
            {
                output.Write("    {0}:", dependency.Key);
                dependency.Value.Describe(output);
                output.WriteLine();
            }
        }

        protected virtual IEnumerable<ITask> RegisterAdditionalDependencies() {
            return new ITask[0];
        }
    }
}