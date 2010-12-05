using System;
using System.Collections.Generic;

namespace Bounce.Framework {
    public class LogOptionCommandLineTranslator : ILogOptionCommandLineTranslator {
        private readonly LogFactoryRegistry LogFactoryRegistry;
        public const string LogLevelParameter = "loglevel";
        public const string CommandOutputParameter = "command-output";
        public const string LogFormatParameter = "logformat";
        public const string DescribeTasksParameter = "describe-tasks";

        public LogOptionCommandLineTranslator(LogFactoryRegistry logFactoryRegistry) {
            LogFactoryRegistry = logFactoryRegistry;
        }

        public LogOptionCommandLineTranslator() : this(LogFactoryRegistry.Default) { }

        public void ParseCommandLine(ParsedCommandLineParameters parsedParameters, IBounce bounce) {
            parsedParameters.IfParameterDo(LogLevelParameter, loglevel => bounce.LogOptions.LogLevel = ParseLogLevel(loglevel));
            parsedParameters.IfParameterDo(CommandOutputParameter, commandOutput => bounce.LogOptions.CommandOutput = ParseBoolOption(commandOutput));
            parsedParameters.IfParameterDo(LogFormatParameter, logformat => bounce.LogFactory = GetLogFactoryByName(logformat));
            parsedParameters.IfParameterDo(DescribeTasksParameter, descTasks => bounce.LogOptions.DescribeTasks = ParseBoolOption(descTasks));
        }

        private LogLevel ParseLogLevel(string loglevel) {
            try {
                return (LogLevel) Enum.Parse(typeof (LogLevel), loglevel, true);
            } catch (Exception) {
                throw new ConfigurationException(String.Format("log level {0} not recognised, try one of {1}", loglevel, String.Join(", ", Enum.GetNames(typeof(LogLevel)))));
            }
        }

        private bool ParseBoolOption(string option) {
            return option.ToLower() == "true";
        }

        private ITaskLogFactory GetLogFactoryByName(string name) {
            return LogFactoryRegistry.GetLogFactoryByName(name);
        }

        public string GenerateCommandLine(IBounce bounce) {
            var args = new List<string>();

            args.Add("/" + DescribeTasksParameter + ":" + bounce.LogOptions.DescribeTasks.ToString().ToLower());
            args.Add("/" + LogLevelParameter + ":" + bounce.LogOptions.LogLevel.ToString().ToLower());
            args.Add("/" + CommandOutputParameter + ":" + bounce.LogOptions.CommandOutput.ToString().ToLower());

            var logFormatName = LogFactoryRegistry.FindNameForLogFactory(bounce.LogFactory);
            if (logFormatName != null) {
                args.Add("/" + LogFormatParameter + ":" + logFormatName);
            }

            return String.Join(" ", args.ToArray());
        }
    }
}