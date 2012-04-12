namespace Bounce.Framework.Obsolete {
    public class NoValueForParameterException : BounceException {
        public NoValueForParameterException(string parameterName) : base(string.Format("parameter `{0}' value not parsed yet", parameterName)) {}
    }
}