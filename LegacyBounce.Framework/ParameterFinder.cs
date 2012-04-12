using System.Collections.Generic;

namespace LegacyBounce.Framework {
    class ParameterFinder : IParameterFinder {
        public IEnumerable<IParameter> FindParametersInTask(IObsoleteTask task) {
            var walker = new TaskWalker();

            var parameters = new HashSet<IParameter>();

            walker.Walk(new TaskDependency(task), null, dependency => {
                var param = dependency.Task as IParameter;
                if (param != null) {
                    parameters.Add(param);
                }
            });

            return parameters;
        }
    }
}