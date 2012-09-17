using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Web.Administration;

namespace Bounce.Framework.Web
{
    public class Iis7 : IWebServer {
        private WebSiteBindingParser WebSiteBindingParser;

        public Iis7() {
            WebSiteBindingParser = new WebSiteBindingParser();
        }

        public IWebSite InstallWebSite(string name, string directory, string binding, params string [] bindings) {
            var iis = new ServerManager();

            var webSiteBindings = new[] {binding}.Concat(bindings).Select(b => WebSiteBindingParser.Parse(b)).ToArray();

            InstallWebSites(name, directory, webSiteBindings.Where(b => b.Path == null), iis);
            InstallWebApplications(directory, webSiteBindings.Where(b => b.Path != null), iis);

            iis.CommitChanges();
            return null;
        }

        private class BindingGroup {
            public string Information;
            public string Protocol;
        }

        private void InstallWebApplications(string directory, IEnumerable<WebSiteBinding> bindings, ServerManager iis) {
            if (bindings.Any()) {
                var bindingGroups = bindings.GroupBy(b => new BindingGroup {Information = b.Information, Protocol = b.Protocol});

                foreach (var bindingGroup in bindingGroups) {
                    var site = iis.Sites.Single(s => s.Bindings.Any(b => b.BindingInformation == bindingGroup.Key.Information && b.Protocol == bindingGroup.Key.Protocol));
                    foreach (var webSiteBinding in bindingGroup) {
                        site.Applications.Add(webSiteBinding.Path, directory);
                    }
                }
            }
        }

        private void InstallWebSites(string name, string directory, IEnumerable<WebSiteBinding> bindings, ServerManager iis) {
            if (bindings.Any()) {
                var webServerBinding = bindings.First();
                var site = iis.Sites.Add(name, webServerBinding.Protocol, webServerBinding.Information, directory);

                foreach (var b in bindings.Skip(1)) {
                    site.Bindings.Add(b.Information, b.Protocol);
                }
            }
        }

        public IWebSite WebSite(string name) {
            return new Iis7WebSite(name);
        }

        public IWebApplication Application(string binding) {
            var webSiteBinding = WebSiteBindingParser.Parse(binding);
            var iis = new ServerManager();
            var sites = iis.Sites.Where(s => s.Bindings.Any(b => b.BindingInformation == webSiteBinding.Information && b.Protocol == webSiteBinding.Protocol)).ToArray();

            if(!sites.Any()) {
                webSiteBinding.Path = null;
                throw new WebSiteNotFoundException(String.Format("could not find website with binding `{0}'", webSiteBinding));
            }

            return new Iis7WebApplication(sites.Single().Name, webSiteBinding.Path);
        }

        public IWebApplication Application(string name, string path) {
            return null;
        }

        public IAppPool AppPool(string name) {
            return null;
        }
    }
}
