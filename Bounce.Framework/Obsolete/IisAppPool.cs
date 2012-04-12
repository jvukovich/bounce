using System;
using System.Management;

namespace Bounce.Framework.Obsolete {
    public class IisAppPool {
        private readonly ManagementScope scope;
        private readonly ManagementObject appPool;
        private readonly ManagementObject appPoolSettings;
        public string Name { get; private set; }

        public Iis6AppPoolIdentity Identity
        {
            get
            {
                var type = (Iis6AppPoolIdentityType) appPoolSettings["AppPoolIdentityType"];
                var identity = new Iis6AppPoolIdentity {IdentityType = type};
                if (identity.HasUserNamePassword)
                {
                    identity.UserName = (string) appPoolSettings["WAMUserName"];
                }

                return identity;
            }
            set
            {
                appPoolSettings["AppPoolIdentityType"] = value.IdentityType;
                if (value.HasUserNamePassword)
                {
                    appPoolSettings["WAMUserName"] = value.UserName;
                    appPoolSettings["WAMUserPass"] = value.Password;
                }
                appPoolSettings.Put();
            }
        }

        public IisAppPool(ManagementScope scope, string name) {
            string path = String.Format("IIsApplicationPool='W3SVC/AppPools/{0}'", name);
            string settingsPath = String.Format("IIsApplicationPoolSetting.Name='W3SVC/AppPools/{0}'", name);
            this.scope = scope;
            appPool = new ManagementObject(scope, new ManagementPath(path), null);
            appPoolSettings = new ManagementObject(scope, new ManagementPath(settingsPath), null);
            Name = name;
        }

        public void Delete() {
            appPool.Delete();
        }

        public void Start() {
            appPool.InvokeMethod("Start", new object[0]);
        }

        public void Stop() {
            appPool.InvokeMethod("Stop", new object[0]);
        }
    }

    public class Iis6AppPoolIdentity {
        private string _userName;
        public string UserName {
            get { return _userName; }
            set {
                _userName = value;
                IdentityType = Iis6AppPoolIdentityType.Configured;
            }
        }

        private string _password;
        public string Password {
            get { return _password; }
            set {
                _password = value;
                IdentityType = Iis6AppPoolIdentityType.Configured;
            }
        }

        public bool HasUserNamePassword
        {
            get {
                return IsConfigured(IdentityType);
            }
        }

        private static bool IsConfigured(Iis6AppPoolIdentityType type)
        {
            return type == Iis6AppPoolIdentityType.Configured;
        }

        private Iis6AppPoolIdentityType _identityType;
        public Iis6AppPoolIdentityType IdentityType {
            get { return _identityType; }
            set {
                _identityType = value;
                if (!IsConfigured(_identityType)) {
                    _userName = null;
                    _password = null;
                }
            }
        }
    }

    public enum Iis6AppPoolIdentityType {
        LocalSystem = 0,
        LocalService = 1,
        NetworkService = 2,
        Configured = 3,
    }
}