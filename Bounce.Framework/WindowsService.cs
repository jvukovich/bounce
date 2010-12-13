using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Bounce.Framework {
    public class WindowsService : Task {
        [Dependency]
        public Future<string> Name;
        [Dependency]
        public Future<string> BinaryPath;
        [Dependency]
        public Future<string> Description;
        [Dependency]
        public Future<string> DisplayName;
        [Dependency]
        public Future<string> UserName;
        [Dependency]
        public Future<string> Password;
        [Dependency]
        public Future<string> Machine;

        public const string NetworkService = @"NT AUTHORITY\NetworkService";

        public WindowsService() {
            Machine = "localhost";
        }

        protected bool IsServiceRunning(IBounce bounce)
        {
            Regex statePattern = new Regex(@"STATE\s+:\s+\d\s+STARTED");

            var queryOutput = ExecuteSc(bounce, @"query ""{0}""", Name.Value).Output;

            return statePattern.Match(queryOutput).Success;
        }

        private ProcessOutput ExecuteSc(IBounce bounce, string commandArgs, params object[] args) {
            return bounce.ShellCommand.Execute("sc", OnMachine(commandArgs, args));
        }

        private void ExecuteScAndExpectSuccess(IBounce bounce, string commandArgs, params object[] args)
        {
            bounce.ShellCommand.ExecuteAndExpectSuccess("sc", OnMachine(commandArgs, args));
        }

        private string OnMachine(string commandArgs, params object [] args) {
            return String.Format(@"\\{0} {1}", Machine.Value, String.Format(commandArgs, args));
        }

        private void StopService(IBounce bounce) {
            ExecuteScAndExpectSuccess(bounce, @"stop ""{0}""", Name.Value);
        }

        public override void Build(IBounce bounce) {
            if (IsServiceInstalled(bounce)) {
                bounce.Log.Info("service {0} installed, deleting", Name.Value);
                DeleteService(bounce);
            }
            InstallService(bounce);
        }

        private void InstallService(IBounce bounce) {
            System.Console.WriteLine("installing service {0}", Name.Value);

            ExecuteScAndExpectSuccess(bounce, @"create ""{0}"" binPath= ""{1}""{2}", Name.Value, Path.GetFullPath(BinaryPath.Value), GetSettings());

            if (Description != null && Description.Value != null) {
                ExecuteScAndExpectSuccess(bounce, @"description ""{0}"" ""{1}""", Name.Value, Description.Value);
            }
        }

        private bool IsServiceInstalled(IBounce bounce)
        {
            return !ExecuteSc(bounce, @"query ""{0}""", Name.Value)
                        .Output
                        .Contains("service does not exist");
        }

        private string GetSettings() {
            return String.Join("", new [] {GetSetting(DisplayName, "DisplayName"), GetSetting(UserName, "obj"), GetSetting(Password, "Password")});
        }

        private string GetSetting(Future<string> setting, string scSetting) {
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
    }
}