using System;

namespace Bounce.Framework {
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class DependencyAttribute : Attribute {}
}