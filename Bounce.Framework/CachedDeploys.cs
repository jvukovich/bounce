using System;
using System.Collections.Generic;

namespace Bounce.Framework {
    public class CachedDeploys {
        private Dictionary<string, ITask> Deploys;
        public CachedDeploys() {
            Deploys = new Dictionary<string, ITask>();
        }

        public ITask Deploy(Task<string> package, Func<Task<string>, ITask> deploy) {
            return package.SelectTask(p =>
            {
                ITask r;

                if (!Deploys.TryGetValue(p, out r))
                {
                    Deploys[p] = r = deploy(package);
                }
                return r;
            });
        }
    }
}