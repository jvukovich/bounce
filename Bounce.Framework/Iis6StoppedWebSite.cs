using System;
using System.Threading;

namespace Bounce.Framework
{
    public class Iis6StoppedWebSite : Iis6Task
    {
        [Dependency]
        public Task<string> Name;
        [Dependency]
        public Task<TimeSpan> Wait;

        public Iis6StoppedWebSite()
        {
            Wait = TimeSpan.FromMilliseconds(0);
        }

        public override void Build()
        {
            IisWebSite webSite = Iis.TryGetWebSiteByServerComment(Name.Value);
            if (webSite != null)
            {
                webSite.Stop();
                Thread.Sleep(Wait.Value);
            }
        }
    }
}