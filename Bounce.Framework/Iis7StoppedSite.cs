using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Web.Administration;

namespace Bounce.Framework
{
    public class Iis7StoppedSite : Task
    {
        [Dependency]
        public Task<string> Name;
        [Dependency]
        public Task<TimeSpan> Wait;

        public Iis7StoppedSite()
        {
            Wait = TimeSpan.FromMilliseconds(0);
        }

        public override void Build()
        {
            var iisServer = new ServerManager();
            var site = iisServer.Sites[Name.Value];

            if (site != null)
            {
                site.Stop();
                Thread.Sleep(Wait.Value);
            }
        }
    }
}
