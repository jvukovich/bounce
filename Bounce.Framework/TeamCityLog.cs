using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

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
            if (Path.GetFileName(command).ToLower() == "msbuild.exe") {
                return new TeamCityMSBuildLogger(args, output);
            } else {
                return new NullCommandLog(args);
            }
        }

        public ITaskLog TaskLog { get; private set; }
    }

    public class TeamCityFormatter {
        private TeamCityTextFormatter TextFormatter;

        public TeamCityFormatter() {
            TextFormatter = new TeamCityTextFormatter();
        }

        public string FormatTeamCityMessage(string name, params string [] fields) {
            var output = new StringBuilder();
            output.Append("##teamcity[" + name);

            for (int i = 0; i < fields.Length - 1; i += 2) {
                output.Append(" " + fields[i] + "='" + TextFormatter.FormatTeamCityText(fields[i + 1]) + "'");
            }

            output.Append("]");

            return output.ToString();
        }
    }

    public class TeamCityTextFormatter {
        public string FormatTeamCityText(string text) {
            return text.Replace("|", "||").Replace("'", "|'").Replace("\n", "|n").Replace("\r", "|r").Replace("]", "|]");
        }
    }

    public class TeamCityMSBuildLogger : ICommandLog {
        private readonly TextWriter stdout;
        private Regex warningRegex;
        private Regex errorRegex;
        private TeamCityFormatter TeamCityFormatter;

        public TeamCityMSBuildLogger(string args, TextWriter stdout) {
            this.stdout = stdout;
            
            Type msBuildLoggerType = typeof(MSBuildLogger);

            string loggerType = String.Format(@"""/l:{0},{1}""", msBuildLoggerType.FullName, msBuildLoggerType.Assembly.Location);

            CommandArgumentsForLogging = args + " " + loggerType + " /nologo /noconsolelogger";
            warningRegex = CreateRegexFor("warning");
            errorRegex = CreateRegexFor("error");

            TeamCityFormatter = new TeamCityFormatter();
        }

        private Regex CreateRegexFor(string type) {
            return new Regex(@"(?<file>.*)\((?<line>\d+),(?<col>\d+)\): " + type + " (?<code>.*?): (?<message>.*)");
        }

        public void CommandOutput(string output) {
            if (output != null) {
                if (warningRegex.IsMatch(output)) {
                    stdout.WriteLine(TeamCityFormatter.FormatTeamCityMessage(
                        "message",
                        "text", output,
                        "status", "WARNING"));
                } else if (errorRegex.IsMatch(output)) {
                    stdout.WriteLine(TeamCityFormatter.FormatTeamCityMessage(
                        "message",
                        "text", output,
                        "status", "ERROR"));
                }
            }
        }

        public void CommandError(string error) {
        }

        public void CommandComplete(int exitCode) {
        }

        public string CommandArgumentsForLogging { get; private set; }
    }
}