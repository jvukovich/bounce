using System.Reflection;

namespace Bounce.Framework.Obsolete {
    public interface ITargetsMethodInvoker {
        object InvokeTargetsMethod(MethodInfo getTargetsMethod, IParameters parameters);
    }
}