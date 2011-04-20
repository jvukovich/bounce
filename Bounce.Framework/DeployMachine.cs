using System.Collections.Generic;

namespace Bounce.Framework {
    public class DeployMachine
    {
        public Task<string> RemotePath;
        public Task<string> LocalPath;
        public Task<string> Machine;
        public IEnumerable<IParameter> BounceParameters = new IParameter[0];
    }
}