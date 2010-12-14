using System;
using System.Collections.Generic;
using System.Linq;

namespace Bounce.Framework {
    class CommandLineTasksParametersGenerator : ICommandLineTasksParametersGenerator {
        private readonly IParameterFinder ParameterFinder;
        private readonly ITypeParsers TypeParsers;

        public CommandLineTasksParametersGenerator(IParameterFinder parameterFinder, ITypeParsers typeParsers) {
            ParameterFinder = parameterFinder;
            TypeParsers = typeParsers;
        }

        public CommandLineTasksParametersGenerator() : this(new ParameterFinder(), Framework.TypeParsers.Default) {
        }

        public string GenerateCommandLineParametersForTasks(IEnumerable<ITask> tasks, IEnumerable<IParameter> overridingParameters) {
            IEnumerable<IParameter> taskParameters = tasks.SelectMany(t => ParameterFinder.FindParametersInTask(t)).Distinct();

            var mergedParameters = OverrideTaskParameters(taskParameters, overridingParameters);

            return GenerateCommandLineParameters(mergedParameters.OrderBy(p => p.Name));
        }

        private IEnumerable<IParameter> OverrideTaskParameters(IEnumerable<IParameter> taskParameters, IEnumerable<IParameter> overridingParameters)
        {
            Dictionary<string, IParameter> parameters = overridingParameters.ToDictionary(p => p.Name);
            var mergedParameters = new List<IParameter>();

            foreach (var taskParameter in taskParameters)
            {
                IParameter param;
                if (parameters.TryGetValue(taskParameter.Name, out param))
                {
                    mergedParameters.Add(param);
                } else
                {
                    mergedParameters.Add(taskParameter);
                }
            }

            return mergedParameters;
        }

        class ParameterComparer : IEqualityComparer<IParameter> {
            public bool Equals(IParameter x, IParameter y) {
                return x.Name == y.Name;
            }

            public int GetHashCode(IParameter p) {
                return p.Name.GetHashCode();
            }
        }

        private string GenerateCommandLineParameters(IEnumerable<IParameter> parameters) {
            return String.Join(" ", parameters.Select(p => p.Generate(TypeParsers)).ToArray());
        }
    }
}