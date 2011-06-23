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
            output.WriteLine(TeamCityFormatter.FormatTeamCityMessage("progressStart", task.SmallDescription));
        }

        public void EndTask(ITask task, IBounceCommand command, TaskResult result) {
            output.WriteLine(TeamCityFormatter.FormatTeamCityMessage("progressFinish", task.SmallDescription));
        }

        public void BeginTarget(ITask task, string name, IBounceCommand command) {
            output.WriteLine(TeamCityFormatter.FormatTeamCityMessage("progressStart", name));
        }

        public void EndTarget(ITask task, string name, IBounceCommand command, TaskResult result) {
            output.WriteLine(TeamCityFormatter.FormatTeamCityMessageWithFields("buildStatus", "status", result == TaskResult.Success? "SUCCESS": "FAILURE"));
            output.WriteLine(TeamCityFormatter.FormatTeamCityMessage("progressFinish", name));
        }
    }
}