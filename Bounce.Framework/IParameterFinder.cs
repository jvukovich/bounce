using System.Collections.Generic;

namespace Bounce.Framework {
    public interface IParameterFinder {
        IEnumerable<IParameter> FindParametersInTask(ITask task);
    }
}