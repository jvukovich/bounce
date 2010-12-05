using System.Reflection;

namespace Bounce.Framework {
    public class TargetsMethodInvoker : ITargetsMethodInvoker {
        public object InvokeTargetsMethod(MethodInfo getTargetsMethod, IParameters parameters) {
            ParameterInfo[] methodParameters = getTargetsMethod.GetParameters();
            if (methodParameters.Length == 1) {
                if (methodParameters[0].ParameterType.IsAssignableFrom(typeof(IParameters)))
                {
                    return getTargetsMethod.Invoke(null, new[] { parameters });
                }
            }

            if (methodParameters.Length == 0) {
                return getTargetsMethod.Invoke(null, new object[0]);
            }

            throw new TargetsMethodWrongSignatureException(getTargetsMethod.Name);
        }
    }
}