namespace Bounce.Framework {
    public class Iis6AppPool : Iis6Task {
        [Dependency]
        public Val<string> Name;

        public override void Build() {
            DeleteIfExtant();

            IisAppPool appPool = Iis.CreateAppPool(Name.Value);
        }

        private void DeleteIfExtant() {
            IisAppPool appPool = Iis.TryGetAppPoolByName(Name.Value);

            if (appPool != null) {
                appPool.Delete();
            }
        }

        public override void Clean() {
            DeleteIfExtant();
        }
    }
}