using Bounce.Framework.Obsolete;

namespace Bounce.Framework {
    public class RequiredParameterNotGivenException : BounceException {
        public RequiredParameterNotGivenException(string name) : base(string.Format("required parameter `{0}' not given", name)) {
        }

        public override void Explain(System.IO.TextWriter stderr)
        {
            base.Explain(stderr);
        }
    }
}