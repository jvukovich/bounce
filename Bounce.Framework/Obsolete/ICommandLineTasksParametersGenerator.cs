using System.Collections.Generic;

namespace Bounce.Framework.Obsolete {
    public interface ICommandLineTasksParametersGenerator {
        string GenerateCommandLineParametersForTasks(IEnumerable<IParameter> parameters, IEnumerable<IParameter> overridingParameters);
    }
}