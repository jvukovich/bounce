using System.Reflection;

namespace LegacyBounce.Framework {
    public interface ITargetsMethodInvoker {
        object InvokeTargetsMethod(MethodInfo getTargetsMethod, IParameters parameters);
    }
}