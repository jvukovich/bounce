using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Bounce.Framework
{
    public interface IDependencyResolver
    {
        object Resolve(Type t);
    }

    internal class AttributedDependencyResolvers : IDependencyResolver
    {
        private readonly List<MethodInfo> _resolvers = new List<MethodInfo>();

        public void AddDependencyResolver(MethodInfo resolver)
        {
            _resolvers.Add(resolver);
        }

        public object Resolve(Type t)
        {
            var instance = _resolvers.Select(x => x.Invoke(null, new object[] {t})).FirstOrDefault(i => i != null);
            return instance ?? t.GetConstructor(new Type[0]).Invoke(new object[0]);
        }
    }
}