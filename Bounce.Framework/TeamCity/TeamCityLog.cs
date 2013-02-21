using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Bounce.Framework.TeamCity {
    public class TeamCityLog : Log {
        private TextWriter Output;
        private TeamCityFormatter TeamCityFormatter;

        public TeamCityLog(LogLevel logLevel) : base(logLevel) {
            Output = Console.Out;
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
            return TeamCityFormatter.FormatTeamCityMessageWithFields("message", "text", message.ToString(), "status", "ERROR");
        }

        private string FormatException(Exception message) {
            return TeamCityFormatter.FormatTeamCityMessageWithFields("message", "text", message.Message, "errorDetails", message.ToString(), "status", "ERROR");
        }

        private void LogErrorMessage(object message) {
            Output.WriteLine(FormatErrorMessage(message));
        }

        private void LogException(object message, Exception exception) {
            LogErrorMessage(message);
            Output.WriteLine(FormatException(exception));
        }

        public virtual ICommandLog NUnitLogger(string args, TextWriter output, ICommandLog log) {
            return new TeamCityNUnitLogger(args, output, log);            
        }
    }
}