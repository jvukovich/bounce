using System.Collections.Generic;

namespace Bounce.Framework.Obsolete {
    class ParameterFinder : IParameterFinder {
        public IEnumerable<IParameter> FindParametersInTask(ITask task) {
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