using System;
using System.Collections.Generic;

namespace Bounce {
    public class IisWebSite : ITarget {
        public IIisWebSiteDirectory WebSiteDirectory;

        #region ITarget Members

        public IEnumerable<ITarget> Dependencies {
            get { return new[] {WebSiteDirectory}; }
        }

        public void Build() {
            Console.WriteLine("installing IIS website at: " + WebSiteDirectory.Path);
            LastBuilt = DateTime.UtcNow;
        }

        public void Clean() {
            Console.WriteLine("uninstalling IIS website at: " + WebSiteDirectory.Path);
        }

        public DateTime? LastBuilt { get; set; }

        #endregion
    }
}