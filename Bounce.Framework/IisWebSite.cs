using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Net;

namespace Bounce.Framework {
    class IisWebSite {
        private readonly ManagementObject webSite;
        private readonly ManagementObject settings;

        public IisWebSite(ManagementScope scope, string path) {
            webSite = new ManagementObject(scope, new ManagementPath(path), null);
            settings = new ManagementObject(scope, new ManagementPath(String.Format("IIsWebServerSetting.Name='{0}'", webSite["Name"])), null);
        }

        public string ServerComment {
            get {
                return (string) settings["ServerComment"];
            }
        }

        public IEnumerable<IisWebSiteBinding> Bindings {
            get {
                var bindings = (ManagementBaseObject[]) settings["ServerBindings"];
                return bindings.Select(b => CreateWebSiteBinding(b)).ToArray();
            }
        }

        public IisWebSiteState State {
            get {
                var state = (int) webSite["ServerState"];
                switch (state) {
                    case 1:
                        return IisWebSiteState.Starting;
                    case 2:
                        return IisWebSiteState.Started;
                    case 3:
                        return IisWebSiteState.Stopping;
                    case 4:
                        return IisWebSiteState.Stopped;
                    case 5:
                        return IisWebSiteState.Pausing;
                    case 6:
                        return IisWebSiteState.Paused;
                    case 7:
                        return IisWebSiteState.Continuing;
                    default:
                        throw new Exception(string.Format("didn't expect website serverstate of {0}", state));
                }
            }
        }

        private static IisWebSiteBinding CreateWebSiteBinding(ManagementBaseObject binding) {
            var ip = (string) binding["IP"];
            var hostname = (string) binding["Hostname"];

            return new IisWebSiteBinding {
                                             Hostname = String.IsNullOrEmpty(hostname)? null: hostname,
                                             IPAddress = (String.IsNullOrEmpty(ip)? null: IPAddress.Parse(ip)),
                                             Port = int.Parse((string) binding["Port"])
                                         };
        }

        public void Delete() {
            webSite.Delete();
        }

        public void Start() {
            webSite.InvokeMethod("Start", new object[0]);
        }

        public void Stop() {
            webSite.InvokeMethod("Stop", new object[0]);
        }
    }
}