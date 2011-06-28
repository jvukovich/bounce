using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bounce.Framework {
    public class BounceFactory {
        public static ITargetBuilderBounce GetBounce() {
            return new Bounce();
        }
    }
}
