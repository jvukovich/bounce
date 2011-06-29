using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bounce.Framework;

namespace Build
{
    public class Build
    {
        [Targets]
        public static object Targets(IParameters parameters)
        {
            var v4 = new VisualStudioSolution {SolutionPath = @"Bounce.sln", Configuration = "Debug"};
            var v35 = new VisualStudioSolution {SolutionPath = @"Bounce.sln", Configuration = "Debug_3_5"};

            return new
            {
                Binaries4 = v4,
                Binaries35 = v35,
                Binaries = new All(v4, v35),
            };
        }
    }
}
