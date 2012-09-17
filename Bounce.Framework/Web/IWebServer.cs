namespace Bounce.Framework.Web {
    public interface IWebServer {
        IWebSite InstallWebSite(string name, string directory, string binding, params string[] bindings);
        IWebSite WebSite(string name);
    }
}