using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Web.Administration;

namespace Bounce.Framework {

    public class Iis7WebSite : Task {
        [Dependency]
        public Task<string> Directory;
        [Dependency]
        public Task<int> Port;
        [Dependency]
        public Task<string> Name;
        [Dependency]
        public Task<IEnumerable<Iis7WebSiteBinding>> Bindings;
        [Dependency]
        public Task<string> ApplicationPoolName;

        public override void Build(IBounce bounce) {
            var iisServer = new ServerManager();

            var existingSite = iisServer.Sites[Name.Value];
            if (!SiteUpToDate(existingSite)) {
                bounce.Log.Info("installing IIS website at: " + Directory.Value);
                RemoveWebSiteIfExtant(iisServer);
                var site = iisServer.Sites.Add(Name.Value, Directory.Value, Port.Value);

                if (Bindings != null && Bindings.Value != null) {
                    site.Bindings.Clear();                    
                    foreach (var binding in Bindings.Value) {
                        site.Bindings.Add(binding.Information.Value, binding.Protocol.Value);
                    }
                }

                if (ApplicationPoolNameIfSet != null) {
                    site.ApplicationDefaults.ApplicationPoolName = ApplicationPoolNameIfSet;
                }

                iisServer.CommitChanges();
            } else {
                bounce.Log.Info("IIS website already installed");
            }
        }

        private bool SiteUpToDate(Site site) {
            if (site != null) {
                if (site.Applications[0].VirtualDirectories[0].PhysicalPath != Directory.Value) {
                    return false;
                }

                if (site.ApplicationDefaults.ApplicationPoolName != ApplicationPoolNameIfSet) {
                    return false;
                }

                if (Bindings == null) {
                    var expectedBindingInformation = String.Format("*:{0}:", Port.Value);
                    if (site.Bindings.Any(b => b.BindingInformation != expectedBindingInformation)) {
                        return false;
                    }
                }
                else {
                    if (site.Bindings.Any(b => {
                        return Bindings.Value.Any(e => e.Information.Value != b.BindingInformation || e.Protocol.Value != b.Protocol);
                    })) {
                        return false;
                    }
                }

                return true;
            } else {
                return false;
            }
        }

        private void RemoveWebSiteIfExtant(ServerManager server) {
            var site = server.Sites[Name.Value];
            if (site != null) {
                server.Sites.Remove(site);
            }
        }

        public override void Clean() {
            var iisServer = new ServerManager();
            RemoveWebSiteIfExtant(iisServer);
            iisServer.CommitChanges();
        }

        public string ApplicationPoolNameIfSet {
            get {
                if (ApplicationPoolName != null && !string.IsNullOrEmpty(ApplicationPoolName.Value))
                    return ApplicationPoolName.Value;

                return null;
            }
        }
    }
}