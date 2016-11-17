using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Bounce.Console {
	[Serializable]
	public class BadExitException : Exception {
		public BadExitException() {}

		public BadExitException(string message) : base(message) {}

		public BadExitException(string message, Exception inner) : base(message, inner) {}

		protected BadExitException(SerializationInfo info, StreamingContext context) : base(info, context) {}

		[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
		public override void GetObjectData(SerializationInfo info, StreamingContext context) {
			if (info == null) throw new ArgumentNullException(nameof(info));
			base.GetObjectData(info, context);
		}
	}
}