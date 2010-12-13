using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Bounce.Framework {
    public static class BounceCommandExtensions
    {
        public static void InvokeCommand(this BounceCommand command, Action build, Action clean)
        {
            switch (command) {
                case BounceCommand.Build:
                    build();
                    break;
                case BounceCommand.Clean:
                    clean();
                    break;
                default:
                    throw new ConfigurationException(String.Format("no such command {0}, try build or clean", command));
            }
        }
    }

    public abstract class Task : ITask {
        public virtual IEnumerable<ITask> Dependencies {
            get {
                return TaskDependencyFinder.Instance.GetDependenciesFor(this).Concat(RegisterAdditionalDependencies());
            }
        }

        public virtual void Invoke(BounceCommand command, IBounce bounce)
        {
            command.InvokeCommand(() => Build(bounce), () => Clean(bounce));
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