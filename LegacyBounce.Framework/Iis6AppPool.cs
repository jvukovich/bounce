using System.IO;

namespace LegacyBounce.Framework {
    public class Iis6AppPool : Iis6Task {
        [Dependency]
        public Task<string> Name;
        [Dependency]
        public Task<Iis6AppPoolIdentity> Identity;

        public Iis6AppPool()
        {
            Identity = new Iis6AppPoolIdentity {IdentityType = Iis6AppPoolIdentityType.NetworkService};
        }

        public override void Build() {
            IisAppPool appPool = CreateIfNotExtant();
            appPool.Identity = Identity.Value;
            appPool.Start();
        }

        private IisAppPool CreateIfNotExtant()
        {
            IisAppPool appPool = Iis.FindAppPoolByName(Name.Value);

            if (appPool == null) {
                appPool = Iis.CreateAppPool(Name.Value);
            }

            return appPool;
        }

        private void DeleteIfExtant() {
            IisAppPool appPool = Iis.FindAppPoolByName(Name.Value);

            if (appPool != null) {
                appPool.Delete();
            }
        }

        public override void Clean() {
            DeleteIfExtant();
        }

        public override void Describe(TextWriter output) {
            output.WriteLine("Iis6AppPool");
            output.WriteLine("Machine: {0}", Machine.Value);
        }
    }
}