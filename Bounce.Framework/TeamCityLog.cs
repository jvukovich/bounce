using System;
using System.IO;

namespace Bounce.Framework {
    class TeamCityLog : ILog {
        private TextWriter output;
        private TeamCityFormatter TeamCityFormatter;

        public TeamCityLog(TextWriter output) {
            TaskLog = new TeamCityTaskLog(output);
            this.output = output;
            TeamCityFormatter = new TeamCityFormatter();
        }

        public void Debug(string format, params object[] args) {
        }

        public void Debug(object message) {
        }

        public void Info(string format, params object[] args) {
        }

        public void Info(object message) {
        }

        public void Warning(string format, params object[] args) {
        }

        public void Warning(Exception exception, string format, params object[] args) {
        }

        public void Warning(object message) {
        }

        public void Warning(Exception exception, object message) {
        }

        public void Error(string format, params object[] args) {
            LogErrorMessage(String.Format(format, args));
        }

        public void Error(Exception exception, string format, params object[] args) {
            LogException(String.Format(format, args), exception);
        }

        public void Error(object message) {
            LogErrorMessage(message);
        }

        public void Error(Exception exception, object message) {
            LogException(message, exception);
        }

        private string FormatErrorMessage(object message) {
            return TeamCityFormatter.FormatTeamCityMessage("message", "text", message.ToString(), "status", "ERROR");
        }

        private string FormatException(Exception message) {
            return TeamCityFormatter.FormatTeamCityMessage("message", "text", message.Message, "errorDetails", message.ToString(), "status", "ERROR");
        }

        private string FormatTeamCityText(string text) {
            throw new NotImplementedException();
        }

        private void LogErrorMessage(object message) {
            output.WriteLine(FormatErrorMessage(message));
        }

        private void LogException(object message, Exception exception) {
            LogErrorMessage(message);
            output.WriteLine(FormatException(exception));
        }

        public ICommandLog BeginExecutingCommand(string command, string args) {
            var commandName = Path.GetFileName(command).ToLower();

            switch (commandName) {
                case "msbuild.exe":
                    return new TeamCityMsBuildLogger(args, output);
                case "nunit-console.exe":
                    return new TeamCityNUnitLogger(args, output);
                default:
                    return new NullCommandLog(args);
            }
        }

        public ITaskLog TaskLog { get; private set; }
    }
}