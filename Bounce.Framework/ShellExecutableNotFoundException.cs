using System;

namespace Bounce.Framework
{
    public class ShellExecutableNotFoundException : BounceException
    {
        public ShellExecutableNotFoundException(string pathToExecutable) : base(String.Format("could not find path for executable: `{0}'", pathToExecutable))
        {
        }
    }
}