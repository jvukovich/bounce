using System.Net;

namespace Bounce.Framework {
    public class Iis6WebSiteBinding {
        [Dependency]
        public Future<string> Hostname;
        [Dependency]
        public Future<int> Port;
        [Dependency]
        public Future<IPAddress> IPAddress;
    }
}