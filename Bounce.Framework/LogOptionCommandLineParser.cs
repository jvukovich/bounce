using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;

namespace Bounce.Framework {
    public class LogOptionCommandLineParser : ILogOptionCommandLineParser {
        public const string LogLevelParameter = "loglevel";
        public const string CommandOutputParameter = "command-output";
        private const string LogFileParameter = "logfile";

        public LogOptions ParseCommandLine(Arguments arguments) {
            var logOptions = new LogOptions {
                LogLevel = arguments.Parameter(LogLevelParameter, LogLevel.Warning),
                CommandOutput = arguments.Parameter(CommandOutputParameter, false),
            };

            SetLogOutput(arguments, logOptions);

            return logOptions;
        }

        private void SetLogOutput(Arguments arguments, LogOptions logOptions) {
            var filename = arguments.Parameter<string>(LogFileParameter, null);

            if (filename != null) {
                var textWriter = File.AppendText(filename);
                textWriter.AutoFlush = true;
                logOptions.StdErr = textWriter;
                logOptions.StdOut = textWriter;
            } else {
                logOptions.StdErr = Console.Error;
                logOptions.StdOut = Console.Out;
            }
        }
    }
}