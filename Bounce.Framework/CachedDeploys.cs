using System;
using System.Collections.Generic;

namespace Bounce.Framework {
    public class CachedDeploys {
        private Dictionary<Task<string>, ITask> RemoteDeploys;
        public CachedDeploys() {
            RemoteDeploys = new Dictionary<Task<string>, ITask>();
        }

        public ITask Deploy(Task<string> package, Func<Task<string>, ITask> remoteDeploy) {
            ITask r;
            if (!RemoteDeploys.TryGetValue(package, out r)) {
                RemoteDeploys[package] = r = remoteDeploy(package);
            }
            return r;
        }
    }
}