using Microsoft.Web.Administration;

namespace Bounce.Framework.Web {
    public class Iis7WebApplication : IWebApplication {
        private readonly string SiteName;
        private readonly string Path;

        public Iis7WebApplication(string siteName, string path) {
            SiteName = siteName;
            Path = path;
        }

        public string Directory { get; set; }

        public bool Exists {
            get {
                var iis = new ServerManager();
                return iis.Sites[SiteName].Applications[Path] != null;
            }
        }

        public void Save() {
            var iis = new ServerManager();

            var site = iis.Sites[SiteName];
            site.Applications.Add(Path, Directory);
            iis.CommitChanges();
        }
    }
}