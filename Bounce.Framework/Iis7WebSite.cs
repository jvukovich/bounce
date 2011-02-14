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

        public override void Build(IBounce bounce) {
            var iisServer = new ServerManager();

            var existingSite = iisServer.Sites[Name.Value];
            if (!SiteUpToDate(existingSite)) {
                bounce.Log.Info("installing IIS website at: " + Directory.Value);
                RemoveWebSiteIfExtant(iisServer);
                iisServer.Sites.Add(Name.Value, Directory.Value, Port.Value);
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

                var expectedBindingInformation = String.Format("*:{0}:", Port.Value);
                if (site.Bindings.All(b => b.BindingInformation != expectedBindingInformation)) {
                    return false;
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
    }
}