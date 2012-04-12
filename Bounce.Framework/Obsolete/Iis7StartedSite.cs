using System;
using System.Threading;
using Microsoft.Web.Administration;

namespace Bounce.Framework.Obsolete
{
    public class Iis7StartedSite : Task
    {
        [Dependency]
        public Task<string> Name;
        [Dependency]
        public Task<TimeSpan> Wait;

        public Iis7StartedSite()
        {
            Wait = TimeSpan.FromMilliseconds(0);
        }

        public override void Build()
        {
            var iisServer = new ServerManager();
            var site = iisServer.Sites[Name.Value];

            if (site != null)
            {
                site.Start();
                Thread.Sleep(Wait.Value);
            }
        }
    }
}
