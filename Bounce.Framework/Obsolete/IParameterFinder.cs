using System.Collections.Generic;

namespace Bounce.Framework.Obsolete {
    public interface IParameterFinder {
        IEnumerable<IParameter> FindParametersInTask(IObsoleteTask task);
    }
}