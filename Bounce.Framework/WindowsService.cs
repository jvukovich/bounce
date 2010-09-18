using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Bounce.Framework {
    public class WindowsService : Task {
        [Dependency]
        public Val<string> Name;
        [Dependency]
        public Val<string> BinaryPath;
        [Dependency]
        public Val<string> Description;
        [Dependency]
        public Val<string> DisplayName;
        [Dependency]
        public Val<string> UserName;
        [Dependency]
        public Val<string> Password;

        private ShellCommandExecutor ShellCommandExecutor;

        public WindowsService() {
            ShellCommandExecutor = new ShellCommandExecutor();
        }

        public override void BeforeBuild() {
            if (Name.Value != null) {
                if (ServiceInstalled && ServiceRunning) {
                    StopService();
                }
            }
        }

        protected bool ServiceRunning {
            get {
                Regex statePattern = new Regex(@"STATE\s+:\s+\d\s+STARTED");

                var queryOutput = ShellCommandExecutor
                    .Execute("sc", String.Format(@"query ""{0}""", Name.Value))
                    .Output;

                return statePattern.Match(queryOutput).Success;
            }
        }

        private void StopService() {
            ShellCommandExecutor.ExecuteAndExpectSuccess("sc", String.Format(@"stop ""{0}""", Name.Value));
        }

        public override void Build() {
            if (ServiceInstalled) {
                DeleteService();
            }
            InstallService();
        }

        private void InstallService() {
            ShellCommandExecutor.ExecuteAndExpectSuccess("sc", String.Format(@"create ""{0}"" binPath= ""{1}""{2}", Name.Value, BinaryPath.Value, GetSettings()));

            if (Description != null && Description.Value != null) {
                ShellCommandExecutor.ExecuteAndExpectSuccess("sc", String.Format(@"description ""{0}"" ""{1}""", Name.Value, Description.Value));
            }
        }

        private bool ServiceInstalled {
            get {
                return !ShellCommandExecutor
                        .Execute("sc", String.Format(@"query ""{0}""", Name.Value))
                        .Output
                        .Contains("service does not exist");
            }
        }

        private string GetSettings() {
            return String.Join("", new [] {GetSetting(DisplayName, "DisplayName"), GetSetting(UserName, "obj"), GetSetting(Password, "Password")});
        }

        private string GetSetting(Val<string> setting, string scSetting) {
            if (setting != null && setting.Value != null) {
                return String.Format(@" {0}= ""{1}""", scSetting, setting.Value);
            } else {
                return "";
            }
        }

        public override void Clean() {
            if (ServiceInstalled) {
                DeleteService();
            }
        }

        private void DeleteService() {
            ShellCommandExecutor.ExecuteAndExpectSuccess("sc", String.Format(@"delete ""{0}""", Name.Value));
        }
    }
}