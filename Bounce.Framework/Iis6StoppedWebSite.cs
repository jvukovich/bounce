namespace Bounce.Framework
{
    public class Iis6StoppedWebSite : Iis6Task
    {
        [Dependency]
        public Future<string> Name;

        public override void Build()
        {
            IisWebSite webSite = Iis.TryGetWebSiteByServerComment(Name.Value);
            if (webSite != null)
            {
                webSite.Stop();
            }
        }
    }
}