using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bounce.Framework.VisualStudio
{
    public class VisualStudio
    {
        public VisualStudioSolution Solution(string path) {
            return new VisualStudioSolution(path);
        }
    }
}
