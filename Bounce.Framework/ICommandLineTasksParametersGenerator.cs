using System.Collections.Generic;

namespace Bounce.Framework {
    public interface ICommandLineTasksParametersGenerator {
        string GenerateCommandLineParametersForTasks(IEnumerable<ITask> tasks, IEnumerable<IParameter> overridingParameters);
    }
}