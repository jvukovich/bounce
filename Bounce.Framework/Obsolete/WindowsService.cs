using System;
using System.IO;

namespace Bounce.Framework.Obsolete {
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
        [Dependency]
        public Task<WindowsServiceStartupType> StartupType;

        [Dependency] public Task<bool> IsServiceStartRequired;

        public const string NetworkService = @"NT AUTHORITY\NetworkService";

        public WindowsService()
        {
            StartupType = WindowsServiceStartupType.Manual;
        }

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
            return String.Join("", new []
                                       {
                                           GetSetting(DisplayName, "DisplayName"),
                                           GetSetting(UserName, "obj"),
                                           GetSetting(Password, "password"),
                                           GetStartSetting(),
                                       });
        }

        private string GetStartSetting()
        {
            return String.Format(@" start= ""{0}""", GetStartupType());
        }

        private string GetStartupType()
        {
            switch(StartupType.Value)
            {
                case WindowsServiceStartupType.Automatic:
                    return "auto";
                case WindowsServiceStartupType.Disabled:
                    return "disabled";
                case WindowsServiceStartupType.Manual:
                    return "demand";
                default:
                    throw new BounceException("windows service startup type not known");
            };
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
}