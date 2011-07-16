using System;
namespace Bounce.Framework {
    interface IBounceFactory {
        ITargetBuilderBounce GetBounce();
        ITargetBuilderBounce GetBounce(LogOptions logOptions);
    }
}
