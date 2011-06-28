using System.Collections.Generic;
using System.Reflection;

namespace Bounce.Framework {
    public interface ITargetsRetriever {
        IDictionary<string, ITask> GetTargetsFromAssembly(MethodInfo getTargetsMethod, IParameters parameters);
        IDictionary<string, ITask> GetTargetsFromObject(object targets);
    }
}