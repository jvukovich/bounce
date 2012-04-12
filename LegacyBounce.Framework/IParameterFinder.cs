using System.Collections.Generic;

namespace LegacyBounce.Framework {
    public interface IParameterFinder {
        IEnumerable<IParameter> FindParametersInTask(IObsoleteTask task);
    }
}