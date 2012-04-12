using System.Collections.Generic;
using System.Reflection;

namespace Bounce.Framework.Obsolete {
    public interface ITargetsRetriever {
        IDictionary<string, IObsoleteTask> GetTargetsFromAssembly(MethodInfo getTargetsMethod, IParameters parameters);
        IDictionary<string, IObsoleteTask> GetTargetsFromObject(object targets);
    }
}