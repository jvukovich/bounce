namespace LegacyBounce.Framework {
    public class Iis7WebSiteBinding {
        [Dependency]
        public Task<string> Protocol;

        [Dependency]
        public Task<string> Information;
    }
}
