using System;

namespace Bounce.Framework.Obsolete
{
    public class On : Task
    {
        public static IObsoleteTask Build(Action onBuild)
        {
            return new On(onBuild);
        }

        private Action OnBuild;

        public On(Action onBuild)
        {
            OnBuild = onBuild;
        }

        public override void Build()
        {
            OnBuild();
        }
    }
}