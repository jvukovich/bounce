using System;
using System.IO;

namespace Bounce.Framework {
    public class TeamCityNUnitLogger : ICommandLog {
        private readonly TextWriter Output;
        private TeamCityFormatter TeamCityFormatter;

        public TeamCityNUnitLogger(string args, TextWriter output) {
            Output = output;
            CommandArgumentsForLogging = args;
            TeamCityFormatter = new TeamCityFormatter();
        }

        public void CommandOutput(string output) {
        }

        public void CommandError(string error) {
        }

        public void CommandComplete(int exitCode) {
            var resultsPath = Path.GetFullPath("TestResult.xml");
            Output.WriteLine(TeamCityFormatter.FormatTeamCityMessage("importData", "type", "nunit", "path", resultsPath));
        }

        public string CommandArgumentsForLogging { get; private set; }
    }
}