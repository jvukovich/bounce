using System;
using System.IO;

namespace Bounce.Console
{
    public abstract class BounceConsoleException : Exception
    {
        public abstract void Explain(TextWriter writer);
    }
}