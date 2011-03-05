using System;
using System.Collections.Generic;

namespace Bounce.Console {
    static class FirstNonNullIEnumerableExtension
    {
        public static TResult FirstNonNull<TItem, TResult>(this IEnumerable<TItem> items, Func<TItem, TResult> f) where TResult : class {
            foreach (var item in items) {
                var result = f(item);
                if (result != null) {
                    return result;
                }
            }

            return null;
        }
    }
}