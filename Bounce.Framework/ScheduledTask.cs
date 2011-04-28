using System;
using System.IO;

namespace Bounce.Framework
{
    public class ScheduledTask : Task
    {
        public Task<string> Machine;
        public Task<string> UserName;
        public Task<string> Password;
        public Task<string> Name;
        public Task<string> BinaryPath;
        public Task<string> SchTasksArguments;
        public Task<bool> ForceDelete;
        public Task<string> SchTasksCommandPath;

        public ScheduledTask()
        {
            SchTasksArguments = "";
            ForceDelete = false;
            SchTasksCommandPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "schtasks.exe");
        }

        public override void Build(IBounce bounce)
        {
            SchTasks(bounce, GetCreateSchTasksArguments());
        }

        public override void Clean(IBounce bounce)
        {
            SchTasks(bounce, GetDeleteSchTasksArguments());
        }

        private void SchTasks(IBounce bounce, string arguments)
        {
            bounce.ShellCommand.ExecuteAndExpectSuccess(SchTasksCommandPath.Value, arguments);
        }

        private string GetCreateSchTasksArguments()
        {
            return String.Format("/Create {0}", GetCreateSettings());
        }

        private string GetDeleteSchTasksArguments()
        {
            return String.Format("/Delete {0}", GetDeleteSettings());
        }

        private string GetCreateSettings() {
            return String.Join("", new []
                                       {
                                           GetSetting(Machine, "S"),
                                           GetSetting(UserName, "RU"),
                                           GetSetting(Password, "RP"),
                                           GetSetting(Name, "TN"),
                                           GetSetting(BinaryPath, "TR"),
                                           SchTasksArguments.Value,
                                       });
        }

        private string GetDeleteSettings() {
            return String.Join("", new []
                                       {
                                           GetSetting(Machine, "S"),
                                           GetSetting(UserName, "RU"),
                                           GetSetting(Password, "RP"),
                                           GetSetting(Name, "TN"),
                                           GetForceSetting(),
                                       });
        }

        private string GetForceSetting()
        {
            return ForceDelete.Value ? "/F" : "";
        }

        private static string GetSetting(Task<string> setting, string schtasksSetting) {
            if (setting != null && setting.Value != null) {
                return String.Format(@" /{0} ""{1}""", schtasksSetting, setting.Value);
            } else {
                return "";
            }
        }
    }
}