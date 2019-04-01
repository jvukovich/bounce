using System;
using Bounce.Framework;

namespace Bounce.Tests.Framework
{
    public class SimpleDependencyResolver : IDependencyResolver
    {
        public object Resolve(Type t)
        {
            return t.GetConstructor(new Type[0]).Invoke(new object[0]);
        }
    }
}