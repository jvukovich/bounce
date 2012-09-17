namespace Bounce.Framework.Web {
    public interface IWebApplication {
        string Directory { get; set; }
        bool Exists { get; }
        void Save();
    }
}