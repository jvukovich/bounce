using System;
using System.Net;

namespace Bounce.Framework.Web {
    public class WebSiteBinding {
        public int Port;
        public string Host;
        public string Path;
        public string Protocol;
        public IPAddress IpAddress;

        public string Information {
            get { return String.Format("{0}:{1}:{2}", IpAddressInformation, Port, Host); }
        }

        private string IpAddressInformation {
            get {
                if (IpAddress != null) {
                    return IpAddress.ToString();
                } else {
                    return "*";
                }
            }
        }

        public override string ToString() {
            bool hidePort = Protocol == "http" && Port == 80 || Protocol == "https" && Port == 443;

            string host = Host == null && IpAddress == null? "*": Host ?? IpAddress.ToString();

            return String.Format("{0}://{1}{2}/{3}", Protocol, host, hidePort? "": ":" + Port, Path ?? "");
        }
    }
}