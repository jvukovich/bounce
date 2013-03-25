using System.Collections.Generic;

namespace Bounce.Framework {
    public interface ITaskRunner {
        void RunTask(string taskName, TaskParameters taskParameters, IEnumerable<ITask> tasks);
    }
}