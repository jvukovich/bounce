using System.IO;

namespace Bounce.Framework {
    class TeamCityTaskLog : ITaskLog {
        private TextWriter output;
        private TeamCityFormatter TeamCityFormatter;

        public TeamCityTaskLog(TextWriter output) {
            this.output = output;
            TeamCityFormatter = new TeamCityFormatter();
        }

        public void BeginTask(ITask task, IBounceCommand command) {
        }

        public void EndTask(ITask task, IBounceCommand command, TaskResult result) {
        }

        public void BeginTarget(ITask task, string name, IBounceCommand command) {
        }

        public void EndTarget(ITask task, string name, IBounceCommand command, TaskResult result) {
            output.WriteLine(TeamCityFormatter.FormatTeamCityMessage("buildStatus", "status", result == TaskResult.Success? "SUCCESS": "FAILURE"));
        }
    }
}