using System;

namespace Bounce.Framework.Tests {
    public class FakeBounce : IBounce {
        public ILog Log { get; set; }
        public IShellCommandExecutor ShellCommand { get; set; }
        public LogOptions LogOptions { get; set; }
        public ITaskLogFactory LogFactory { get; set; }

        public FakeBounce() {
            LogOptions = new LogOptions();
        }

        public virtual void Build(ITask task) { }
        public virtual void Clean(ITask task) { }
    }
}