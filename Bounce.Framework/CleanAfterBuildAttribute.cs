using System;

namespace Bounce.Framework {
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class CleanAfterBuildAttribute : Attribute {}
}