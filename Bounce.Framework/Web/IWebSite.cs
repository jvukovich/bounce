using System.Collections.Generic;

namespace Bounce.Framework.Web {
    public interface IWebSite {
        string Directory { get; set; }
        IList<string> Bindings { get; set; }
        bool Exists { get; }
        void Save();
    }
}