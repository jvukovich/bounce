using System;
using System.Collections.Generic;
using System.Linq;

namespace Bounce.Framework.Obsolete {
    class CommandLineTasksParametersGenerator : ICommandLineTasksParametersGenerator {
        private readonly IParameterFinder ParameterFinder;
        private readonly ITypeParsers TypeParsers;

        public CommandLineTasksParametersGenerator(IParameterFinder parameterFinder, ITypeParsers typeParsers) {
            ParameterFinder = parameterFinder;
            TypeParsers = typeParsers;
        }

        public CommandLineTasksParametersGenerator() : this(new ParameterFinder(), Obsolete.TypeParsers.Default) {
        }

        public string GenerateCommandLineParametersForTasks(IEnumerable<IParameter> parameters, IEnumerable<IParameter> overridingParameters) {
            var mergedParameters = OverrideTaskParameters(parameters, overridingParameters);

            return GenerateCommandLineParameters(mergedParameters.OrderBy(p => p.Name));
        }

        private IEnumerable<IParameter> OverrideTaskParameters(IEnumerable<IParameter> taskParameters, IEnumerable<IParameter> overridingParameters) {
            var mergedParameters = new Dictionary<string, IParameter>(taskParameters.ToDictionary(p => p.Name));

            foreach (var overridingParameter in overridingParameters) {
                mergedParameters[overridingParameter.Name] = overridingParameter;
            }

            return mergedParameters.Values;
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