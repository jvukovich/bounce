using System.Net;

namespace Bounce.Framework.Obsolete {
    public class Iis6WebSiteBinding {
        [Dependency]
        public Task<string> Hostname { get; set; }

        [Dependency]
        public Task<int> Port { get; set; }

        [Dependency]
        public Task<IPAddress> IPAddress { get; set; }

        public Iis6WebSiteBinding() {
            Port = 80;
        }
    }
}