using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Web.Administration;

namespace Bounce.Framework {
    public class Iis7WebSite : Task {
        [Dependency]
        public Val<string> Path;
        [Dependency]
        public Val<int> Port;
        [Dependency]
        public Val<string> Name;

        public override void Build() {
            var iisServer = new ServerManager();

            var existingSite = iisServer.Sites[Name.Value];
            if (!SiteUpToDate(existingSite)) {
                Console.WriteLine("installing IIS website at: " + Path.Value);
                RemoveWebSiteIfExtant(iisServer);
                iisServer.Sites.Add(Name.Value, Path.Value, Port.Value);
                iisServer.CommitChanges();
            } else {
                Console.WriteLine("IIS website already installed");
            }
        }

        private bool SiteUpToDate(Site site) {
            if (site != null) {
                if (site.Applications[0].VirtualDirectories[0].PhysicalPath != Path.Value) {
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