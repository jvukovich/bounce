using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;

namespace Bounce.Framework {
    public class WindowsService : WindowsServiceBaseTask {
        [Dependency]
        public Task<string> BinaryPath;
        [Dependency]
        public Task<string> Description;
        [Dependency]
        public Task<string> DisplayName;
        [Dependency]
        public Task<string> UserName;
        [Dependency]
        public Task<string> Password;

        [Dependency] public Task<bool> IsServiceStartRequired;

        public const string NetworkService = @"NT AUTHORITY\NetworkService";

        protected override void BuildTask(IBounce bounce) {
            if (IsServiceInstalled(bounce)) {
                bounce.Log.Info("service {0} installed, deleting", Name.Value);
                DeleteService(bounce);
            }
            InstallService(bounce);

            if (IsServiceStartRequired.Value) {
                bounce.Log.Info("starting {0} service", Name.Value);
                StartService(bounce);
            }

        }

        private void InstallService(IBounce bounce) {
            System.Console.WriteLine("installing service {0}", Name.Value);

            ExecuteScAndExpectSuccess(bounce, @"create ""{0}"" binPath= ""{1}""{2}", Name.Value, Path.GetFullPath(BinaryPath.Value), GetSettings());

            if (Description != null && Description.Value != null) {
                ExecuteScAndExpectSuccess(bounce, @"description ""{0}"" ""{1}""", Name.Value, Description.Value);
            }
        }

        private string GetSettings() {
            return String.Join("", new [] {GetSetting(DisplayName, "DisplayName"), GetSetting(UserName, "obj"), GetSetting(Password, "Password")});
        }

        private static string GetSetting(Task<string> setting, string scSetting) {
            if (setting != null && setting.Value != null) {
                return String.Format(@" {0}= ""{1}""", scSetting, setting.Value);
            } else {
                return "";
            }
        }

        public override void Clean(IBounce bounce) {
            if (IsServiceInstalled(bounce)) {
                bounce.Log.Info("service {0} installed, deleting", Name.Value);
                DeleteService(bounce);
            } else {
                bounce.Log.Info("service {0} not installed");
            }
        }

        private void DeleteService(IBounce bounce) {
            ExecuteScAndExpectSuccess(bounce, @"delete ""{0}""", Name.Value);
        }

        private void StartService(IBounce bounce) {
            ExecuteScAndExpectSuccess(bounce, @"start ""{0}""", Name.Value);
        }
    }

    public class StoppedWindowsService : WindowsServiceBaseTask
    {
        public Task<TimeSpan> Timeout;

        public StoppedWindowsService()
        {
            Timeout = TimeSpan.FromMinutes(5);
        }

        protected override void BuildTask(IBounce bounce)
        {
            if (IsServiceInstalled(bounce))
                 StopService(bounce);

            DateTime timeWhenAskedToStop = DateTime.Now;

            do
            {
                Thread.Sleep(1000);

                if (WaitedLongerThanTimeout(timeWhenAskedToStop))
                {
                    throw new BounceException(String.Format("service {0} could not be stopped", Name));
                }
            } while (!IsServiceStopped(bounce));
        }

        private bool WaitedLongerThanTimeout(DateTime timeWhenAskedToStop)
        {
            return DateTime.Now.Subtract(timeWhenAskedToStop).CompareTo(Timeout.Value) > 0;
        }
    }

    public abstract class WindowsServiceBaseTask : Task
    {
        [Dependency]
        public Task<string> Name;

        [Dependency]
        public Task<string> Machine;

        protected WindowsServiceBaseTask()
        {
            Machine = "localhost";
        }

        protected abstract void BuildTask(IBounce bounce);
        
        public override void Build(IBounce bounce)
        {
            BuildTask(bounce);
        }

        protected void ExecuteScAndExpectSuccess(IBounce bounce, string commandArgs, params object[] args)
        {
            bounce.ShellCommand.ExecuteAndExpectSuccess("sc", OnMachine(commandArgs, args));
        }

        protected string OnMachine(string commandArgs, params object [] args) {
            return String.Format(@"\\{0} {1}", Machine.Value, String.Format(commandArgs, args));
        }

        protected void StopService(IBounce bounce) {
            ExecuteScAndExpectSuccess(bounce, @"stop ""{0}""", Name.Value);
        }

        protected ProcessOutput ExecuteSc(IBounce bounce, string commandArgs, params object[] args) {
            return bounce.ShellCommand.Execute("sc", OnMachine(commandArgs, args));
        }

        protected bool IsServiceStarted(IBounce bounce)
        {
            return IsServiceStatus(bounce, "started");
        }

        protected bool IsServiceStopped(IBounce bounce)
        {
            return IsServiceStatus(bounce, "stopped");
        }

        private bool IsServiceStatus(IBounce bounce, string status)
        {
            Regex statePattern = new Regex(@"STATE\s+:\s+\d\s+" + status.ToUpper(), RegexOptions.IgnoreCase);
            var queryOutput = ExecuteSc(bounce, @"query ""{0}""", Name.Value).Output;
            return statePattern.Match(queryOutput).Success;
        }

        protected bool IsServiceInstalled(IBounce bounce)
        {
            return !ExecuteSc(bounce, @"query ""{0}""", Name.Value)
                        .Output
                        .Contains("service does not exist");
        }
    }
}