using System;
using System.Collections.Generic;
using System.IO;

namespace Bounce.Framework.Tests {
    public class FakeTask : ITask {
        public IEnumerable<TaskDependency> Dependencies { get; set; }

        public virtual void Build(IBounce bounce) { }
        public virtual void Clean(IBounce bounce) { }
        public void Invoke(IBounceCommand command, IBounce bounce) { }

        public virtual bool IsLogged { get { return true; } }

        public virtual void Describe(TextWriter output) {}
        
        public string SmallDescription { get { return ""; }}
    }
}