using System;

namespace Bounce.Framework {
    public interface ITargetBuilderBounce : IBounce {
        IDisposable LogForTask(ITask task);
    }
}