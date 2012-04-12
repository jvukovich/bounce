namespace LegacyBounce.Framework
{
    public class Iis6VirtualDirectory : Iis6VirtualDirectorySettings
    {
        [Dependency]
        public Task<string> WebSiteName;
        [Dependency]
        public Task<string> VirtualDirectoryPath;

        public override void Build()
        {
            var webSite = Iis.TryGetWebSiteByServerComment(WebSiteName.Value);
            if (webSite == null)
            {
                throw new TaskException(this, "no such IIS 6 Website: " + WebSiteName.Value);
            }

            var virtualDirectory = webSite.CreateVirtualDirectory(VirtualDirectoryPath.Value, Directory.Value);

            SetupVirtualDirectory(virtualDirectory);
        }

        public override void Clean() {
            var webSite = Iis.TryGetWebSiteByServerComment(WebSiteName.Value);
            if (webSite == null) {
                throw new TaskException(this, "no such IIS 6 Website: " + WebSiteName.Value);
            }

            webSite.DeleteVirtualDirectory(VirtualDirectoryPath.Value);
        }
    }
}