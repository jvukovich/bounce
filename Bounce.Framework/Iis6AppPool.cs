using System;
using System.IO;

namespace Bounce.Framework {
    public class Iis6AppPool : Iis6Task {
        [Dependency]
        public Future<string> Name;
        [Dependency]
        public Future<Iis6AppPoolIdentity> Identity;

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