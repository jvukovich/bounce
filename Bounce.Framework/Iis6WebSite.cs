using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Bounce.Framework
{
    public class Iis6WebSite : Iis6Task
    {
        private static Iis6ScriptMap[] _mvcScriptMaps;
        [Dependency] public Task<Iis6WebSiteAccessFlags> AccessFlags;
        [Dependency] public Iis6AppPool AppPool;
        [Dependency] public Task<IEnumerable<Iis6Authentication>> Authentication;
        [Dependency] public Task<string> Directory;
        [Dependency] public Task<string> Name;
        [Dependency] public Task<IEnumerable<Iis6WebSiteBinding>> Bindings;
        [Dependency] public Task<IEnumerable<Iis6ScriptMap>> ScriptMapsToAdd;
        [Dependency] public Task<bool> Started;

        public Iis6WebSite()
        {
            Bindings = new[] {new Iis6WebSiteBinding {Port = 80}};
            ScriptMapsToAdd = new Iis6ScriptMap[0];
            Authentication = new[] {Iis6Authentication.Basic, Iis6Authentication.NTLM};
            AppPool = null;
            Started = true;
            AccessFlags = Iis6WebSiteAccessFlags.Read;
        }

        public static Iis6ScriptMap[] MvcScriptMaps
        {
            get
            {
                if (_mvcScriptMaps == null)
                {
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

        public override void Build(IBounce bounce)
        {
            DeleteIfExtant();
            IisWebSite webSite = Iis.CreateWebSite(Name.Value, ToInternalBindings(Bindings.Value), Path.GetFullPath(Directory.Value));

            webSite.AddScriptMapsToSite(ScriptMapsToAdd.Value);

            if (AppPool != null)
            {
                webSite.AppPoolName = AppPool.Name.Value;
            }
            
            //This needs to be set after the AppPool, otherwise the authentication settings are ignored
            webSite.Authentication = Authentication.Value;

            webSite.IisWebSiteAccessPermissions = AccessFlags.Value;

            if (Started.Value)
            {
                webSite.Start();
            }
        }

        private static IEnumerable<IisWebSiteBindingDetails> ToInternalBindings(IEnumerable<Iis6WebSiteBinding> bindings)
        {
            return bindings.Select(b => ToInternalBinding(b));
        }

        private static IisWebSiteBindingDetails ToInternalBinding(Iis6WebSiteBinding binding)
        {
            if (binding.Port == null)
            {
                throw new ConfigurationException("Iis 6 port must not be null");
            }

            return new IisWebSiteBindingDetails
                   {
                       Hostname = binding.Hostname == null? null: binding.Hostname.Value,
                       IPAddress = binding.IPAddress == null ? null : binding.IPAddress.Value,
                       Port = binding.Port.Value,
                   };
        }

        private void DeleteIfExtant()
        {
            IisWebSite website = Iis.TryGetWebSiteByServerComment(Name.Value);

            if (website != null)
            {
                website.Delete();
            }
        }

        public override void Clean()
        {
            DeleteIfExtant();
        }

        protected override IEnumerable<TaskDependency> RegisterAdditionalDependencies() {
            return Bindings.Value.SelectMany(b => TaskDependencyFinder.Instance.GetDependenciesFor(b));
        }
    }
}