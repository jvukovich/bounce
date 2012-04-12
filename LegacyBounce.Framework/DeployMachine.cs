using System.Collections.Generic;

namespace LegacyBounce.Framework {
    public interface IDeployMachine
    {
        Task<string> RemotePath { get; }
        IEnumerable<IParameter> BounceParameters { get; }
    }

    public class DeployMachine : IDeployMachine
    {
        public Task<string> RemotePath { get; set; }
        public Task<string> LocalPath;
        public Task<string> Machine;
        public IEnumerable<IParameter> BounceParameters { get; set; }

        public DeployMachine()
        {
            BounceParameters = new IParameter[0];
        }
    }
}