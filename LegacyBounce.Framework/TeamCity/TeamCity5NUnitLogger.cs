using System.IO;
using System.Text.RegularExpressions;

namespace LegacyBounce.Framework.TeamCity {
    public class TeamCity5NUnitLogger : TeamCityNUnitLogger {
        private readonly TextWriter Output;
        private readonly TeamCityFormatter TeamCityFormatter;
        private readonly Regex SummaryRegex;

        public TeamCity5NUnitLogger(string args, TextWriter output, ICommandLog log) 
            : base(args, output, log)
        {
            Output = output;
            TeamCityFormatter = new TeamCityFormatter();

            SummaryRegex = new Regex(@"Tests run: (\d.*), Errors: (\d.*), Failures: (\d.*), Inconclusive: (\d.*), Time: (\d.*) seconds");
        }

        public new void CommandOutput(string output) {
            OutputInconclusiveTestCount(output);

            base.CommandOutput(output);
        }

        private void OutputInconclusiveTestCount(string output) {
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