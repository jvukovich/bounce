using System;

namespace Bounce.Framework.Tests
{
    public class SimpleDependencyResolver : IDependencyResolver
    {
        public object Resolve(Type t)
        {
            return t.GetConstructor(new Type[0]).Invoke(new object[0]);
        }
    }
}