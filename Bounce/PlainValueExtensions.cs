namespace Bounce.Framework {
    public static class PlainValueExtensions {
        public static IValue<T> V<T>(this T s) {
            return new PlainValue<T>(s);
        }
    }
}