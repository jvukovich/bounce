using System.Net;

namespace Bounce.Framework {
    public class Iis6WebSiteBinding {
        [Dependency]
        public Future<string> Hostname { get; set; }

        [Dependency]
        public Future<int> Port { get; set; }

        [Dependency]
        public Future<IPAddress> IPAddress { get; set; }

        public Iis6WebSiteBinding() {
            Port = 80;
        }
    }
}