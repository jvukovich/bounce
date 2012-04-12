using System.Collections.Generic;

namespace LegacyBounce.Framework
{
    public class Iis6VirtualDirectorySettings : Iis6Task
    {
        private static Iis6ScriptMap[] _mvcScriptMaps;
        [Dependency]
        public Task<Iis6WebSiteAccessFlags> AccessFlags;
        [Dependency]
        public Iis6AppPool AppPool;
        [Dependency]
        public Task<IEnumerable<Iis6Authentication>> Authentication;
        [Dependency]
        public Task<string> Directory;
        [Dependency]
        public Task<IEnumerable<Iis6ScriptMap>> ScriptMapsToAdd;

        public static Iis6ScriptMap[] MvcScriptMaps {
            get {
                if (_mvcScriptMaps == null) {
                    _mvcScriptMaps = new[]
                                     {
                                         new Iis6ScriptMap
                                         {
                                             Executable =
                                                 @"c:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\aspnet_isapi.dll",
                                             Extension = ".mvc",
                                         },
                                         new Iis6ScriptMap
                                         {
                                             Executable =
                                                 @"c:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\aspnet_isapi.dll",
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

        protected void SetupVirtualDirectory(IisVirtualDirectorySettings virtualDirectory)
        {
            virtualDirectory.AddScriptMapsToSite(ScriptMapsToAdd.Value);

            if (AppPool != null) {
                virtualDirectory.AppPoolName = AppPool.Name.Value;
            }

            virtualDirectory.IisWebSiteAccessPermissions = AccessFlags.Value;
            virtualDirectory.Authentication = Authentication.Value;

            virtualDirectory.Commit();
        }
    }
}