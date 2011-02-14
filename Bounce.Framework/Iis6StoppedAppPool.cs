namespace Bounce.Framework
{
    public class Iis6StoppedAppPool : Iis6Task
    {
        [Dependency]
        public Future<string> Name;

        public override void Build() {
            IisAppPool appPool = Iis.FindAppPoolByName(Name.Value);
            if (appPool != null) {
                appPool.Stop();
            }
        }
    }
}