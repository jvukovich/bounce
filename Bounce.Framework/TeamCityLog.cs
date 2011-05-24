using System;
using System.IO;

namespace Bounce.Framework {
    class TeamCityLog : Log {
        private TextWriter Output;
        private readonly LogOptions LogOptions;
        private TeamCityFormatter TeamCityFormatter;

        public TeamCityLog(TextWriter output, LogOptions logOptions, ILogMessageFormatter logMessageFormatter) : base(output, output, logOptions, logMessageFormatter) {
            this.Output = output;
            LogOptions = logOptions;
            TeamCityFormatter = new TeamCityFormatter();
        }

        public override void Error(string format, params object[] args) {
            base.Error(format, args);
            LogErrorMessage(String.Format(format, args));
        }

        public override void Error(Exception exception, string format, params object[] args) {
            base.Error(exception, format, args);
            LogException(String.Format(format, args), exception);
        }

        public override void Error(object message) {
            base.Error(message);
            LogErrorMessage(message);
        }

        public override void Error(Exception exception, object message) {
            base.Error(exception, message);
            LogException(message, exception);
        }

        private string FormatErrorMessage(object message) {
            return TeamCityFormatter.FormatTeamCityMessage("message", "text", message.ToString(), "status", "ERROR");
        }

        private string FormatException(Exception message) {
            return TeamCityFormatter.FormatTeamCityMessage("message", "text", message.Message, "errorDetails", message.ToString(), "status", "ERROR");
        }

        private void LogErrorMessage(object message) {
            Output.WriteLine(FormatErrorMessage(message));
        }

        private void LogException(object message, Exception exception) {
            LogErrorMessage(message);
            Output.WriteLine(FormatException(exception));
        }

        public override ICommandLog BeginExecutingCommand(string command, string args) {
            var commandName = Path.GetFileName(command).ToLower();

            ICommandLog log = base.BeginExecutingCommand(command, args);

            switch (commandName) {
                case "msbuild.exe":
                    return new TeamCityMsBuildLogger(args, Output, log);
                case "nunit-console.exe":
                    return new TeamCityNUnitLogger(args, Output, log);
                default:
                    return log;
            }
        }
    }
}