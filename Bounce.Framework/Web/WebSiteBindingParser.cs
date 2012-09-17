using System.Net;
using System.Text.RegularExpressions;

namespace Bounce.Framework.Web {
    public class WebSiteBindingParser {
        private static Regex UrlRegex = new Regex(@"(?<protocol>http|https)://((?<ipaddress>\d+\.\d+\.\d+\.\d+)|(?<host>[^:]*))(:(?<port>\d+))?/(?<path>.*)");

        public WebSiteBinding Parse(string httpHost) {
            var match = UrlRegex.Match(httpHost);
            if (match.Success) {
                return new WebSiteBinding {
                    Port = ParsePort(match),
                    Path = ParsePath(match),
                    Host = ParseHost(match),
                    Protocol = ParseProtocol(match),
                    IpAddress = ParseIpAddress(match)
                };
            }
            return null;
        }

        private IPAddress ParseIpAddress(Match match) {
            if (match.Groups["ipaddress"].Success) {
                return IPAddress.Parse(match.Groups["ipaddress"].Value);
            } else {
                return null;
            }
        }

        private string ParsePath(Match match) {
            if (match.Groups["path"].Value != "") {
                return "/" + match.Groups["path"].Value;
            } else {
                return null;
            }
        }

        private static int ParsePort(Match match) {
            if (match.Groups["port"].Success) {
                return int.Parse(match.Groups["port"].Value);
            } else {
                if (ParseProtocol(match) == "http") {
                    return 80;
                } else {
                    return 443;
                }
            }
        }

        private static string ParseProtocol(Match match) {
            return match.Groups["protocol"].Value;
        }

        private static string ParseHost(Match match) {
            var host = match.Groups["host"].Value;
            return (host == "*" || host == "")? null: host;
        }
    }
}