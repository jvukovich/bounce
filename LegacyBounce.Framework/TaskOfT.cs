using System.Collections.Generic;
using System.IO;

namespace LegacyBounce.Framework {
    public abstract class Task<T> : IObsoleteTask {
        public abstract T Value { get; }
        public abstract IEnumerable<TaskDependency> Dependencies { get; }
        public abstract void Invoke(IBounceCommand command, IBounce bounce);

        public static implicit operator Task<T>(T v) {
            return new ImmediateValue<T>(v);
        }

        public virtual bool IsLogged { get { return false; } }

        public virtual void Describe(TextWriter output) { }

        public string SmallDescription {
            get
            {
                var value = (object) Value;
                if (value != null)
                {
                    return Value.ToString();
                } else
                {
                    return "(null)";
                }
            }
        }

        public virtual bool IsPure { get { return true; } }
    }
}