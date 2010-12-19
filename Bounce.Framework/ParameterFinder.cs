using System;
using System.Collections.Generic;

namespace Bounce.Framework {
    class ParameterFinder : IParameterFinder {
        public IEnumerable<IParameter> FindParametersInTask(ITask task) {
            var walker = new TaskWalker();

            var parameters = new HashSet<IParameter>();

            walker.Walk(new TaskDependency {Task = task}, null, dependency => {
                var param = dependency.Task as IParameter;
                if (param != null) {
                    parameters.Add(param);
                }
            });

            return parameters;
        }
    }
}