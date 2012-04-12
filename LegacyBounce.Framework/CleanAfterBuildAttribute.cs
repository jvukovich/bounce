using System;

namespace LegacyBounce.Framework {
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class CleanAfterBuildAttribute : Attribute {}
}