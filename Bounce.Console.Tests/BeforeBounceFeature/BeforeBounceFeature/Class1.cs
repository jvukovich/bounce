using System;
using Bounce.Framework;
using Bounce.Framework.Tests.Features;

namespace BeforeBounceFeature
{
    public class Build
    {
        [Targets]
        public static object GetTargets() {
            return new {
                OldBeforeBounceFeature = new PrintTask {Description = "building before bounce feature"},
            };
        }
    }
}
