using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Web.Administration;

namespace Bounce.Framework.Iis
{
    public class Iis7 : IIis {
/*        public string Directory;
        public int Port;
        public string Name;
        public IEnumerable<Iis7WebSiteBinding> Bindings;
        public string ApplicationPoolName;

        /// <summary>
        /// Installs or updates a website and places it at the specified bindings
        /// </summary>
        /// <param name="name">Website name as seen in IIS Manager, only used in websites at root level</param>
        /// <param name="directory">Directory of website contents</param>
        /// <param name="bindings">an array of bindings. Bindings take the form [http|https]://host[:port]/[path]</param>
        public void WebSite(string name, string directory, params string[] bindings)
        {
            var iisServer = new ServerManager();

            var existingSite = iisServer.Sites[name];
            if (!SiteUpToDate(existingSite))
            {
                Bounce.Log.Info("installing IIS website at: " + directory);
                RemoveWebSiteIfExtant(iisServer);

                var site = iisServer.Sites.Add(name, directory, 80);

                if (Bindings != null && Bindings.Value != null)
                {
                    site.Bindings.Clear();
                    foreach (var binding in Bindings.Value)
                    {
                        site.Bindings.Add(binding.Information.Value, binding.Protocol.Value);
                    }
                }

                if (ApplicationPoolNameIfSet != null)
                {
                    site.ApplicationDefaults.ApplicationPoolName = ApplicationPoolNameIfSet;
                }

                iisServer.CommitChanges();
            }
            else
            {
                Bounce.Log.Info("IIS website already installed");
            }
        }

        private bool SiteUpToDate(Site site)
        {
            if (site != null)
            {
                if (site.Applications[0].VirtualDirectories[0].PhysicalPath != Directory.Value)
                {
                    return false;
                }

                if (site.ApplicationDefaults.ApplicationPoolName != ApplicationPoolNameIfSet)
                {
                    return false;
                }

                if (Bindings == null)
                {
                    var expectedBindingInformation = String.Format("*:{0}:", Port.Value);
                    if (site.Bindings.Any(b => b.BindingInformation != expectedBindingInformation))
                    {
                        return false;
                    }
                }
                else
                {
                    if (site.Bindings.Any(b => Bindings.Value.Any(e => e.Information.Value != b.BindingInformation || e.Protocol.Value != b.Protocol)))
                    {
                        return false;
                    }
                }

                return true;
            }

            return false;
        }

        private void RemoveWebSiteIfExtant(ServerManager server)
        {
            var site = server.Sites[Name.Value];
            if (site != null)
            {
                server.Sites.Remove(site);
            }
        }

        public override void Clean()
        {
            var iisServer = new ServerManager();
            RemoveWebSiteIfExtant(iisServer);
            iisServer.CommitChanges();
        }

        public string ApplicationPoolNameIfSet
        {
            get
            {
                if (ApplicationPoolName != null && !string.IsNullOrEmpty(ApplicationPoolName.Value))
                    return ApplicationPoolName.Value;

                return null;
            }
        } */
    }
}
