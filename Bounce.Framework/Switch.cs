using System;
using System.Collections.Generic;

namespace Bounce.Framework {
    public class Switch<T> : Task
    {
        [Dependency]
        public Task<T> Condition { get; set; }

        private Dictionary<T, ITask> Cases;

        public Switch(Task<T> condition)
        {
            Condition = condition;
            Cases = new Dictionary<T, ITask>();
        }

        public ITask this [T _case]
        {
            get { return Cases[_case]; }
            set { Cases[_case] = value; }
        }

        public override void Invoke(IBounceCommand command, IBounce bounce)
        {
            ITask action;
            if (Cases.TryGetValue(Condition.Value, out action)) {
                bounce.Invoke(command, action);
            } else {
                throw new ConfigurationException(String.Format("no such case for `{0}'", Condition.Value));
            }
        }

        public override bool IsLogged
        {
            get { return false; }
        }
    }
}