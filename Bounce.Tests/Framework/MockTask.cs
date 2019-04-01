using System.Collections.Generic;
using Bounce.Framework;

namespace Bounce.Tests.Framework
{
    public class MockTask : ITask
    {
        public string FullName { get; set; }
        public IEnumerable<ITaskParameter> Parameters { get; set; }
        public TaskParameters WasInvokedWithTaskParameters { get; private set; }
        public bool WasInvoked { get; private set; }

        public void Invoke(TaskParameters taskParameters)
        {
            WasInvoked = true;
            WasInvokedWithTaskParameters = taskParameters;
        }
    }
}