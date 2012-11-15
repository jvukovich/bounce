using System;
using System.Reflection;

namespace Bounce.Framework {
    public interface IDependencyResolver {
        object Resolve(Type t);
    }
}