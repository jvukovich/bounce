namespace Bounce.Framework {
    class Iis6WebSite : Task {
        [Dependency]
        public Val<string> Directory;
        [Dependency]
        public Val<int> Port;
        [Dependency]
        public Val<string> Name;

        private IisWebServer _iis;

        public override void Build() {
            DeleteIfExtant();
            Iis.CreateWebSite(Name.Value, new[] {new IisWebSiteBinding {Port = Port.Value}}, Directory.Value);
        }

        private IisWebServer Iis {
            get {
                if (_iis == null) {
                    _iis = new IisWebServer("localhost");
                }
                return _iis;
            }
        }

        private void DeleteIfExtant() {
            IisWebSite website = Iis.TryGetWebSiteByServerComment(Name.Value);

            if (website != null) {
                website.Delete();
            }
        }

        public override void Clean() {
            DeleteIfExtant();
        }
    }
}