using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bounce.Framework {
    public class BounceFactory : IBounceFactory {
        public ITargetBuilderBounce GetBounce() {
            return new Bounce();
        }

        public ITargetBuilderBounce GetBounce(ITaskLogFactory taskLogFactory, LogOptions logOptions) {
            return new Bounce(taskLogFactory, logOptions);
        }
    }
}
