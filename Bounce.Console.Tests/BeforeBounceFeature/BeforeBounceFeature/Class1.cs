using System;
using Bounce.Framework;

namespace BeforeBounceFeature
{
    public class Build
    {
        [Task]
        public void BeforeBounceFeature()
        {
            Console.WriteLine("building before bounce feature");
        }
    }
}
