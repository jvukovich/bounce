using System.Collections.Generic;
using System.Linq;

namespace Bounce.Framework {
    public class Iis6WebSite : Task {
        [Dependency]
        public Val<string> Directory;
        [Dependency]
        public Val<int> Port;
        [Dependency]
        public Val<string> Name;
        [Dependency]
        public Val<IEnumerable<Iis6ScriptMap>> ScriptMapsToAdd;
        [Dependency]
        public Val<IEnumerable<Iis6Authentication>> Authentication;

        private IisWebServer _iis;

        private static Iis6ScriptMap[] _mvcScriptMaps = null;

        public static Iis6ScriptMap[] MvcScriptMaps {
            get {
                if (_mvcScriptMaps == null) {
                    _mvcScriptMaps = new[] {
                        new Iis6ScriptMap {
                            Executable = @"c:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\aspnet_isapi.dll",
                            Extension = ".mvc",
                        },
                        new Iis6ScriptMap {
                            Executable = @"c:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\aspnet_isapi.dll",
                            Extension = "*",
                            ScriptEngine = false,
                            IncludedVerbs = "All",
                            VerifyThatFileExists = false,
                        },
                    };
                }
                return _mvcScriptMaps;
            }
        }

        public override void Build() {
            DeleteIfExtant();
            IisWebSite webSite = Iis.CreateWebSite(Name.Value, new[] {new IisWebSiteBinding {Port = Port.Value}}, Directory.Value);
            if (ScriptMapsToAdd != null) {
                IEnumerable<Iis6ScriptMap> scriptMaps = ScriptMapsToAdd.Value;
                if (scriptMaps != null) {
                    webSite.AddScriptMapsToSite(scriptMaps);
                }
            }
            if (Authentication != null) {
                var auth = Authentication.Value;
                if (auth != null) {
                    webSite.Authentication = auth;
                }
            }
        }

        private IisWebServer Iis {
            get {
                if (_iis == null) {
                    _iis = new IisWebServer("localhost");
                }
                return _iis;
            }
        }

        private void DeleteIfExtant() {
            IisWebSite website = Iis.TryGetWebSiteByServerComment(Name.Value);

            if (website != null) {
                website.Delete();
            }
        }

        public override void Clean() {
            DeleteIfExtant();
        }
    }
}