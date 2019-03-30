using System.Collections.Generic;
using System.IO;

namespace Bounce.Framework
{
    public interface ITask
    {
        string FullName { get; }
        IEnumerable<ITaskParameter> Parameters { get; }
        void Invoke(TaskParameters taskParameters);
    }
}