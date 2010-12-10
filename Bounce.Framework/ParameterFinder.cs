using System;
using System.Collections.Generic;

namespace Bounce.Framework {
    class ParameterFinder : IParameterFinder {
        public IEnumerable<IParameter> FindParametersInTask(ITask task) {
            var walker = new TaskWalker();

            var parameters = new HashSet<IParameter>();

            walker.Walk(task, null, maybeParam => {
                if (maybeParam is IParameter) {
                    parameters.Add(maybeParam as IParameter);
                }
            });

            return parameters;
        }
    }
}