using System.Collections.Generic;
using Bounce.Framework.Obsolete;

namespace Bounce.Framework.Tests.Obsolete {
    public class FakeBounce : IBounce {
        public ILog Log { get; set; }
        public IShellCommandExecutor ShellCommand { get; set; }
        public LogOptions LogOptions { get; set; }
        public ITaskLogFactory LogFactory { get; set; }

        public FakeBounce() {
            LogOptions = new LogOptions();
        }

        public IEnumerable<IParameter> ParametersGiven { get; set; }

        public virtual void Invoke(IBounceCommand command, ITask task)
        {
            task.Invoke(command, this);
        }
    }
}