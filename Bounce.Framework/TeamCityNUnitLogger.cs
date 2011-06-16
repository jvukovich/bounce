using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Bounce.Framework {
    public class TeamCityNUnitLogger : ICommandLog {
        private readonly TextWriter Output;
        private readonly ICommandLog Log;
        private readonly TeamCityFormatter TeamCityFormatter;
        private readonly Regex SummaryRegex;
        private readonly bool InconclusiveTestCountRequired;

        public TeamCityNUnitLogger(string args, TextWriter output, ICommandLog log) {
            Output = output;
            Log = log;
            CommandArgumentsForLogging = args;
            TeamCityFormatter = new TeamCityFormatter();

            SummaryRegex = new Regex(@"Tests run: (\d.*), Errors: (\d.*), Failures: (\d.*), Inconclusive: (\d.*), Time: (\d.*) seconds");
            InconclusiveTestCountRequired = DetermineIfInconclusiveTestCountRequired();
        }

        public void CommandOutput(string output)
        {
            OutputInconclusiveTestCount(output);

            Log.CommandOutput(output);
        }

        public void CommandError(string error) {
            Log.CommandError(error);
        }

        public void CommandComplete(int exitCode) {
            var resultsPath = Path.GetFullPath("TestResult.xml");
            Output.WriteLine(TeamCityFormatter.FormatTeamCityMessageWithFields("importData", "type", "nunit", "path", resultsPath));
            Log.CommandComplete(exitCode);
        }

        public string CommandArgumentsForLogging { get; private set; }

        private static bool DetermineIfInconclusiveTestCountRequired()
        {
            var version = Environment.GetEnvironmentVariable("TEAMCITY_VERSION");
            if (version == null) return false;
            
            var regex = new Regex(@"([0-9].*)\.([0-9].*)\.([0-9].*) \(build \d.*\)");
            var match = regex.Match(version);
            
            if (match.Success)
            {
                return (int.Parse(match.Groups[1].Value) < 6);
            }
            return true;
        }

        private void OutputInconclusiveTestCount(string output)
        {
            if (!InconclusiveTestCountRequired) return;
            if (output == null) return;
            var match = SummaryRegex.Match(output);
            
            if (match.Success && int.Parse(match.Groups[4].Value) > 0)
            {
                    Output.WriteLine(TeamCityFormatter.FormatTeamCityMessageWithFields("buildStatus", "text",
                        "{build.status.text}, inconclusive: " + match.Groups[4].Value));
            }
        }
    }
}