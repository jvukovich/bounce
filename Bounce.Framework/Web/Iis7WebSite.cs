using System.Collections.Generic;
using System.Linq;
using Microsoft.Web.Administration;

namespace Bounce.Framework.Web {
    public class Iis7WebSite : IWebSite {
        private readonly string Name;

        public Iis7WebSite(string name) {
            Name = name;
            Bindings = new List<string>();
        }

        public Iis7WebSite(Site site) {
            Name = site.Name;
            Bindings = new List<string>();
        }

        public string Directory { get; set; }
        public IList<string> Bindings { get; set; }

        private IEnumerable<WebSiteBinding> WebSiteBindings {
            get {
                var parser = new WebSiteBindingParser();
                return Bindings.Select(parser.Parse).ToArray();
            }
        }

        public bool Exists {
            get {
                var iis = new ServerManager();
                return iis.Sites[Name] != null;
            }
        }

        public void Save() {
            var iis = new ServerManager();

            var webServerBinding = WebSiteBindings.First();
            var site = iis.Sites.Add(Name, webServerBinding.Protocol, webServerBinding.Information, Directory);

            foreach (var b in WebSiteBindings.Skip(1)) {
                site.Bindings.Add(b.Information, b.Protocol);
            }

            iis.CommitChanges();
        }
    }
}