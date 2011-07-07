using System.IO;
using System;
using System.Text.RegularExpressions;

namespace Bounce.Framework.TeamCity {
    internal class TeamCityLogFactory : ITaskLogFactory {
        public ILog CreateLogForTask(ITask task, TextWriter stdout, TextWriter stderr, LogOptions logOptions) {
            if (TeamCityVersion < 6)
                return new TeamCity5Log(stdout, logOptions, new TaskLogMessageFormatter(task));
            return new TeamCityLog(stdout, logOptions, new TaskLogMessageFormatter(task));
        }

        private static int TeamCityVersion
        {
            get
            {
                var version = Environment.GetEnvironmentVariable("TEAMCITY_VERSION");
                if (version == null) return 0;

                var regex = new Regex(@"([0-9]*)\.([0-9]*)\.?([0-9]*) \(build \d.*\)");
                var match = regex.Match(version);

                if (match.Success)
                {
                    return int.Parse(match.Groups[1].Value);
                }
                return 0;
            }
        }

    }
}