using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Bounce.Console
{
    [Serializable]
    public class TargetsAssemblyNotFoundException : Exception
    {
        public TargetsAssemblyNotFoundException(string message) : base(message)
        {
        }

        protected TargetsAssemblyNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new ArgumentNullException(nameof(info));

            base.GetObjectData(info, context);
        }
    }
}