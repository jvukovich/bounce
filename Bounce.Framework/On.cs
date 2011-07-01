using System;

namespace Bounce.Framework
{
    public class On : Task
    {
        public static ITask Build(Action onBuild)
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