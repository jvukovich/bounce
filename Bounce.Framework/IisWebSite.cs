using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Net;

namespace Bounce.Framework {
    public class IisWebSite {
        private readonly ManagementScope scope;
        private readonly ManagementObject webSite;
        private readonly ManagementObject settings;
        private readonly ManagementObject virtualDirectory;
        private readonly string Name;

        public IisWebSite(ManagementScope scope, string path) {
            this.scope = scope;
            webSite = new ManagementObject(scope, new ManagementPath(path), null);
            Name = (string) webSite["Name"];
            settings = new ManagementObject(scope, new ManagementPath(String.Format("IIsWebServerSetting.Name='{0}'", Name)), null);
            virtualDirectory = new ManagementObject(scope, new ManagementPath(String.Format(@"IIsWebVirtualDirSetting.Name=""{0}/root""", Name)), null);
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

        public void AddScriptMapsToSite(IEnumerable<Iis6ScriptMap> scriptMapsToAdd) {
            if (scriptMapsToAdd.Count() > 0) {
                var scriptMaps = (ManagementBaseObject[])virtualDirectory["ScriptMaps"];

                var newScriptMaps = new ManagementBaseObject[scriptMaps.Length + scriptMapsToAdd.Count()];
                scriptMaps.CopyTo(newScriptMaps, 0);

                int newScriptMapsIndex = scriptMaps.Length;

                foreach (var scriptMap in scriptMapsToAdd) {
                    ManagementObject newScriptMap = CreateScriptMap(scriptMap);
                    newScriptMaps[newScriptMapsIndex++] = newScriptMap;
                }

                virtualDirectory["ScriptMaps"] = newScriptMaps;
                virtualDirectory.Put();
            }
        }

        public IEnumerable<Iis6Authentication> Authentication {
            get {
                var authenticationTypes = new List<Iis6Authentication>();

                foreach(var authType in (Iis6Authentication[]) Enum.GetValues(typeof(Iis6Authentication))) {
                    if ((bool) virtualDirectory[GetAuthenticationProperty(authType)]) {
                        authenticationTypes.Add(authType);
                    }
                }

                return authenticationTypes;
            }
            set {
                foreach(var authType in (Iis6Authentication[]) Enum.GetValues(typeof(Iis6Authentication))) {
                    virtualDirectory[GetAuthenticationProperty(authType)] = value.Contains(authType);
                }

                virtualDirectory.Put();
            }
        }

        public string AppPoolName {
            get {
                return (string) virtualDirectory["AppPoolId"];
            }
            set {
                virtualDirectory["AppPoolId"] = value;
                virtualDirectory.Put();
            }
        }

        private string GetAuthenticationProperty(Iis6Authentication auth) {
            switch(auth) {
                case Iis6Authentication.Anonymous:
                    return "AuthAnonymous";
                case Iis6Authentication.Basic:
                    return "AuthBasic";
                case Iis6Authentication.Digest:
                    return "AuthMD5";
                case Iis6Authentication.DotNetPassport:
                    return "AuthPassport";
                case Iis6Authentication.NTLM:
                    return "AuthNTLM";
                default:
                    throw new ConfigurationException("authentication type not expected: " + auth);
            }
        }

        private ManagementObject CreateScriptMap(Iis6ScriptMap scriptMap) {
            ManagementObject newScriptMap = new ManagementClass(scope, new ManagementPath("ScriptMap"), null).CreateInstance();
            newScriptMap["Extensions"] = scriptMap.Extension;
            newScriptMap["Flags"] = scriptMap.Flags;
            newScriptMap["IncludedVerbs"] = scriptMap.IncludedVerbs;
            newScriptMap["ScriptProcessor"] = scriptMap.Executable;
            return newScriptMap;
        }
    }
}