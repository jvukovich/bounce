using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;

namespace Bounce.Framework
{
    public class IisVirtualDirectorySettings
    {
        private ManagementObject VirtualDirectorySettings;
        private ManagementScope Scope;

        public IisVirtualDirectorySettings(ManagementScope scope, string path)
        {
            Scope = scope;
            VirtualDirectorySettings = new ManagementObject(scope,
                                                            new ManagementPath(
                                                                string.Format("IIsWebVirtualDirSetting.Name='{0}'", path)),
                                                            null);
        }

        public void Commit()
        {
            VirtualDirectorySettings.Put();
        }

        public void AddScriptMapsToSite(IEnumerable<Iis6ScriptMap> scriptMapsToAdd) {
            if (scriptMapsToAdd.Count() > 0) {
                var scriptMaps = (ManagementBaseObject[])VirtualDirectorySettings["ScriptMaps"];

                var newScriptMaps = new ManagementBaseObject[scriptMaps.Length + scriptMapsToAdd.Count()];
                scriptMaps.CopyTo(newScriptMaps, 0);

                int newScriptMapsIndex = scriptMaps.Length;

                foreach (var scriptMap in scriptMapsToAdd) {
                    ManagementObject newScriptMap = CreateScriptMap(scriptMap);
                    newScriptMaps[newScriptMapsIndex++] = newScriptMap;
                }

                VirtualDirectorySettings["ScriptMaps"] = newScriptMaps;
            }
        }

        public IEnumerable<Iis6Authentication> Authentication {
            get {
                var authenticationTypes = new List<Iis6Authentication>();

                foreach (var authType in (Iis6Authentication[])Enum.GetValues(typeof(Iis6Authentication))) {
                    if ((bool)VirtualDirectorySettings[GetAuthenticationProperty(authType)]) {
                        authenticationTypes.Add(authType);
                    }
                }

                return authenticationTypes;
            }
            set {
                foreach (var authType in (Iis6Authentication[])Enum.GetValues(typeof(Iis6Authentication))) {
                    VirtualDirectorySettings[GetAuthenticationProperty(authType)] = value.Contains(authType);
                }
            }
        }

        public Iis6WebSiteAccessFlags IisWebSiteAccessPermissions {
            get {
                return (Iis6WebSiteAccessFlags)VirtualDirectorySettings["AccessFlags"];
            }
            set {
                VirtualDirectorySettings["AccessFlags"] = (int)value;
            }
        }

        public string AppPoolName {
            get {
                return (string)VirtualDirectorySettings["AppPoolId"];
            }
            set {
                VirtualDirectorySettings["AppPoolId"] = value;
            }
        }

        public string Path
        {
            get { return (string) VirtualDirectorySettings["Path"]; }
            set { VirtualDirectorySettings["Path"] = value; }
        }

        private string GetAuthenticationProperty(Iis6Authentication auth) {
            switch (auth) {
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
            ManagementObject newScriptMap = new ManagementClass(Scope, new ManagementPath("ScriptMap"), null).CreateInstance();
            newScriptMap["Extensions"] = scriptMap.Extension;
            newScriptMap["Flags"] = scriptMap.Flags;
            newScriptMap["IncludedVerbs"] = scriptMap.IncludedVerbs;
            newScriptMap["ScriptProcessor"] = scriptMap.Executable;
            return newScriptMap;
        }
    }
}