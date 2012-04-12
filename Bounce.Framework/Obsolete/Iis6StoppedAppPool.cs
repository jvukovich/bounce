using System;
using System.Threading;

namespace Bounce.Framework.Obsolete
{
    public class Iis6StoppedAppPool : Iis6Task
    {
        [Dependency]
        public Task<string> Name;
        [Dependency]
        public Task<TimeSpan> Wait;

        public Iis6StoppedAppPool()
        {
            Wait = TimeSpan.FromMilliseconds(0);
        }

        public override void Build() {
            IisAppPool appPool = Iis.FindAppPoolByName(Name.Value);
            if (appPool != null) {
                appPool.Stop();
                Thread.Sleep(Wait.Value);
            }
        }
    }
}