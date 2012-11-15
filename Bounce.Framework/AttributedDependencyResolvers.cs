using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Bounce.Framework {
    class AttributedDependencyResolvers : IDependencyResolver {
        public List<MethodInfo> Resolvers = new List<MethodInfo>();

        public void AddDependencyResolver(MethodInfo resolver) {
            Resolvers.Add(resolver);
        }

        public object Resolve(Type t) {
            object instance = Resolvers.Select(r => r.Invoke(null, new object[] {t})).FirstOrDefault(i => i != null);

            if (instance != null) {
                return instance;
            } else {
                return t.GetConstructor(new Type[0]).Invoke(new object[0]);
            }
        }
    }
}