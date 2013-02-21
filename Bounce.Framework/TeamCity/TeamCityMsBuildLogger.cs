using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace Bounce.Framework.TeamCity {
    public class TeamCityMsBuildLogger : IShellLogger
    {
        private readonly TextWriter StdOut;
        private TextWriter StdErr;
        private Regex warningRegex;
        private Regex errorRegex;
        private TeamCityFormatter TeamCityFormatter;

        public TeamCityMsBuildLogger() {
            StdOut = Console.Out;
            StdErr = Console.Error;

            warningRegex = CreateRegexFor("warning");
            errorRegex = CreateRegexFor("error");

            TeamCityFormatter = new TeamCityFormatter();
        }

        private Regex CreateRegexFor(string type) {
            return new Regex(@"(?<file>.*)\((?<line>\d+),(?<col>\d+)\): " + type + " (?<code>.*?): (?<message>.*)");
        }

        public string CommandArgumentsForLogging { get; private set; }
        public void OutputDataReceived(object sender, DataReceivedEventArgs e) {
            if (e.Data != null)
            {
                if (warningRegex.IsMatch(e.Data))
                {
                    StdOut.WriteLine(TeamCityFormatter.FormatTeamCityMessageWithFields(
                        "message",
                        "text", e.Data,
                        "status", "WARNING"));
                }
                else if (errorRegex.IsMatch(e.Data))
                {
                    StdOut.WriteLine(TeamCityFormatter.FormatTeamCityMessageWithFields(
                        "message",
                        "text", e.Data,
                        "status", "ERROR"));
                }
            }
        }

        public void ErrorDataReceived(object sender, DataReceivedEventArgs e) {
            StdErr.WriteLine(e.Data);
        }
    }
}