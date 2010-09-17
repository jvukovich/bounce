using System;
using System.Collections.Generic;

namespace Bounce.Framework {
    public class IisWebSite : ITarget {
        public IIisWebSiteDirectory WebSiteDirectory;

        public IEnumerable<ITarget> Dependencies {
            get { return new[] {WebSiteDirectory}; }
        }

        public void Build() {
            Console.WriteLine("installing IIS website at: " + WebSiteDirectory.Path.Value);
            LastBuilt = DateTime.UtcNow;
        }

        public void Clean() {
            Console.WriteLine("uninstalling IIS website at: " + WebSiteDirectory.Path.Value);
        }

        public DateTime? LastBuilt { get; set; }
    }
}