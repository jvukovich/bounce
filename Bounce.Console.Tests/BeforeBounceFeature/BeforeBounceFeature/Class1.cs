using System;
using Bounce.Framework.Obsolete;
using Bounce.Framework.Tests.Obsolete.Features;

namespace BeforeBounceFeature
{
    public class Build
    {
        [Targets]
        public static object GetTargets() {
            return new {
                BeforeBounceFeature = new PrintTask {Description = "building before bounce feature"},
            };
        }
    }
}
