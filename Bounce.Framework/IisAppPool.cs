using System;
using System.Management;

namespace Bounce.Framework {
    public class IisAppPool {
        private readonly ManagementScope scope;
        private readonly ManagementObject appPool;
        public string Name { get; private set; }

        public IisAppPool(ManagementScope scope, string name) {
            string path = String.Format("IIsApplicationPool='W3SVC/AppPools/{0}'", name);
            this.scope = scope;
            appPool = new ManagementObject(scope, new ManagementPath(path), null);
            Name = name;
        }

        public void Delete() {
            appPool.Delete();
        }
    }
}