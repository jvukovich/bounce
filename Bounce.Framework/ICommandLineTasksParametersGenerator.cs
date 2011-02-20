using System.Collections.Generic;

namespace Bounce.Framework {
    public interface ICommandLineTasksParametersGenerator {
        string GenerateCommandLineParametersForTasks(IEnumerable<IParameter> parameters, IEnumerable<IParameter> overridingParameters);
    }
}