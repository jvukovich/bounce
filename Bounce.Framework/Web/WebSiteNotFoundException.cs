using System;

namespace Bounce.Framework.Web {
    public class WebSiteNotFoundException : Exception {
        public WebSiteNotFoundException(string message) : base(message) {}
    }
}