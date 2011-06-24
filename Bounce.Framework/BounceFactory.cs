using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bounce.Framework
{
    public class BounceFactory
    {
        public static IBounce CreateBounce()
        {
            return new Bounce();
        }
    }
}
