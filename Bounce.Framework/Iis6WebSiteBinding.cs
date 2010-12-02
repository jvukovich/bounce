using System.Net;

namespace Bounce.Framework {
    public class Iis6WebSiteBinding {
        [Dependency]
        public Val<string> Hostname;
        [Dependency]
        public Val<int> Port;
        [Dependency]
        public Val<IPAddress> IPAddress;
    }
}