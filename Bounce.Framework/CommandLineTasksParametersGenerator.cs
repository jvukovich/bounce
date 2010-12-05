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

        public string GenerateCommandLineParametersForTasks(IEnumerable<ITask> tasks) {
            IEnumerable<IParameter> parameters = tasks.SelectMany(t => ParameterFinder.FindParametersInTask(t)).Distinct();

            return String.Join(" ", parameters.Select(p => p.Generate(TypeParsers)).ToArray());
        }
    }
}