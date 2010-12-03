using System;
using System.IO;

namespace Bounce.Framework {
    public interface ITargetBuilderBounce : IBounce {
        ITaskScope TaskScope(ITask task, BounceCommand command, string targetName);
        TextWriter DescriptionOutput { get; }
    }
}