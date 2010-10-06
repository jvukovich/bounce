using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Bounce.Framework {
    public class TeamCityMsBuildLogger : ICommandLog {
        private readonly TextWriter stdout;
        private Regex warningRegex;
        private Regex errorRegex;
        private TeamCityFormatter TeamCityFormatter;

        public TeamCityMsBuildLogger(string args, TextWriter stdout) {
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