using System.Collections.Generic;
using System.Reflection;

namespace Bounce.Framework {
    public static class MethodInfoExtensions {
        public static IEnumerable<string> TaskNames(this MethodInfo methodInfo) {
            var fullName = methodInfo.DeclaringType.FullName + "." + methodInfo.Name;

            yield return fullName;
            int index = fullName.IndexOf(".");
            while (index > 0) {
                fullName = fullName.Substring(index + 1);
                yield return fullName;
                index = fullName.IndexOf(".");
            }
        }
    }
}