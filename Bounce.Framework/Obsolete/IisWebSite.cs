using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Net;

namespace Bounce.Framework.Obsolete {
    public class IisWebSite {
        private readonly ManagementScope scope;
        private readonly ManagementObject webSite;
        private readonly ManagementObject settings;
        private readonly string Name;
        public IisVirtualDirectorySettings VirtualDirectory { get; private set; }

        public IisWebSite(ManagementScope scope, string path) {
            this.scope = scope;
            webSite = new ManagementObject(scope, new ManagementPath(path), null);
            Name = (string) webSite["Name"];
            settings = new ManagementObject(scope, new ManagementPath(String.Format("IIsWebServerSetting.Name='{0}'", Name)), null);
            VirtualDirectory = new IisVirtualDirectorySettings(scope, Name + "/root");
        }

        public string ServerComment {
            get {
                return (string) settings["ServerComment"];
            }
        }

        public IEnumerable<IisWebSiteBindingDetails> Bindings {
            get {
                var bindings = (ManagementBaseObject[]) settings["ServerBindings"];
                return bindings.Select(b => CreateWebSiteBinding(b)).ToArray();
            }
        }

        public IisVirtualDirectorySettings CreateVirtualDirectory(string subPath, string directory) {
            var path = String.Format(@"{0}/root/{1}", Name, subPath);

            var vDir = new ManagementClass(scope, new ManagementPath("IIsWebVirtualDirSetting"), null).CreateInstance();
            vDir["Name"] = path;
            vDir["Path"] = directory;
            vDir.Put();

            var virtualDirPath = string.Format("IIsWebVirtualDir.Name='{0}'", path);
            var app = new ManagementObject(scope, new ManagementPath(virtualDirPath), null);
            app.InvokeMethod("AppCreate2", new object[] { 2 });

            return new IisVirtualDirectorySettings(scope, path);
        }

        public void DeleteVirtualDirectory(string subPath) {
            var name = String.Format(@"{0}/root/{1}", Name, subPath);
            var path = string.Format("IIsWebVirtualDir.Name='{0}'", name);
            var virtualDirectory = new ManagementObject(scope, new ManagementPath(path), null);
            virtualDirectory.Delete();
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

        public IisVirtualDirectorySettings CreateVirtualDirectory(string name)
        {
            return null;
        }

        private static IisWebSiteBindingDetails CreateWebSiteBinding(ManagementBaseObject binding) {
            var ip = (string) binding["IP"];
            var hostname = (string) binding["Hostname"];

            return new IisWebSiteBindingDetails {
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