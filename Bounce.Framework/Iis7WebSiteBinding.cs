using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bounce.Framework {
    public class Iis7WebSiteBinding {
        [Dependency]
        public Task<string> Protocol;

        [Dependency]
        public Task<string> Information;
    }
}
