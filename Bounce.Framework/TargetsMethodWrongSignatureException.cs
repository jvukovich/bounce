using System.IO;

namespace Bounce.Framework {
    internal class TargetsMethodWrongSignatureException : BounceException {
        private readonly string methodName;

        public TargetsMethodWrongSignatureException(string methodName) {
            this.methodName = methodName;
        }

        public override void Explain(TextWriter writer) {
            writer.WriteLine("method with [Targets] attribute has wrong signature. Try one of these:");
            writer.WriteLine();
            writer.WriteLine(
                @"    public static object {0} (IParameters parameters) {{}}
    public static object {0} () {{}}",
                methodName);
        }
    }
}