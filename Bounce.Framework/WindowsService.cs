using System;
using System.Collections.Generic;
using System.IO;
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
        [Dependency]
        public Val<string> Machine;

        public const string NetworkService = @"NT AUTHORITY\NetworkService";

        private ShellCommandExecutor ShellCommandExecutor;

        public WindowsService() {
            ShellCommandExecutor = new ShellCommandExecutor();
            Machine = "localhost";
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

                var queryOutput = ExecuteSc(@"query ""{0}""", Name.Value).Output;

                return statePattern.Match(queryOutput).Success;
            }
        }

        private ProcessOutput ExecuteSc(string commandArgs, params object[] args) {
            return ShellCommandExecutor.Execute("sc", OnMachine(commandArgs, args));
        }

        private void ExecuteScAndExpectSuccess(string commandArgs, params object[] args) {
            ShellCommandExecutor.ExecuteAndExpectSuccess("sc", OnMachine(commandArgs, args));
        }

        private string OnMachine(string commandArgs, params object [] args) {
            return String.Format(@"\\{0} {1}", Machine.Value, String.Format(commandArgs, args));
        }

        private void StopService() {
            ExecuteScAndExpectSuccess(@"stop ""{0}""", Name.Value);
        }

        public override void Build(IBounce bounce) {
            if (ServiceInstalled) {
                bounce.Log.Info("service {0} installed, deleting", Name.Value);
                DeleteService();
            }
            InstallService();
        }

        private void InstallService() {
            Console.WriteLine("installing service {0}", Name.Value);

            ExecuteScAndExpectSuccess(@"create ""{0}"" binPath= ""{1}""{2}", Name.Value, Path.GetFullPath(BinaryPath.Value), GetSettings());

            if (Description != null && Description.Value != null) {
                ExecuteScAndExpectSuccess(@"description ""{0}"" ""{1}""", Name.Value, Description.Value);
            }
        }

        private bool ServiceInstalled {
            get {
                return !ExecuteSc(@"query ""{0}""", Name.Value)
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

        public override void Clean(IBounce bounce) {
            if (ServiceInstalled) {
                bounce.Log.Info("service {0} installed, deleting", Name.Value);
                DeleteService();
            } else {
                Console.WriteLine("service {0} not installed");
            }
        }

        private void DeleteService() {
            ExecuteScAndExpectSuccess(@"delete ""{0}""", Name.Value);
        }
    }
}