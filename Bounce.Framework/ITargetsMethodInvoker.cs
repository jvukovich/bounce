using System.Reflection;

namespace Bounce.Framework {
    public interface ITargetsMethodInvoker {
        object InvokeTargetsMethod(MethodInfo getTargetsMethod, IParameters parameters);
    }
}