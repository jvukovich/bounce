using System;
using System.IO;
using System.Runtime.Serialization;

namespace Bounce.Framework
{
    [Serializable]
    public class BounceException : Exception
    {
        public BounceException(string message) : base(message)
        {
        }

        public BounceException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public BounceException()
        {
        }

        public BounceException(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext)
        {
        }

        public virtual void Explain(TextWriter stderr)
        {
            stderr.WriteLine(Message);
        }
    }
}