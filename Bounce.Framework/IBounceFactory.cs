using System;
namespace Bounce.Framework {
    public interface IBounceFactory {
        ITargetBuilderBounce GetBounce();
        ITargetBuilderBounce GetBounce(LogOptions logOptions);
    }
}
