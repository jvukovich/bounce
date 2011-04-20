using System;
using System.Collections.Generic;

namespace Bounce.Framework {
    class Switch : Task
    {
        [Dependency]
        public Task<string> Condition { get; set; }

        private IDictionary<string, ITask> Cases;

        public Switch(Task<string> condition)
        {
            Condition = condition;
            Cases = new Dictionary<string, ITask>();
        }

        public ITask this [string _case]
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