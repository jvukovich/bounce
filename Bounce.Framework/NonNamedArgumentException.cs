namespace Bounce.Framework
{
    public class NonNamedArgumentException : BounceException
    {
        private readonly string _arg;

        public NonNamedArgumentException(string arg)
        {
            _arg = arg;
        }

        public override void Explain(System.IO.TextWriter stderr)
        {
            stderr.WriteLine($"expected switch argument beginning with '/', found '{_arg}'");
        }
    }
}