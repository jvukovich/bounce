using System.Collections.Generic;

namespace LegacyBounce.Framework {
    public interface ICommandLineTasksParametersGenerator {
        string GenerateCommandLineParametersForTasks(IEnumerable<IParameter> parameters, IEnumerable<IParameter> overridingParameters);
    }
}