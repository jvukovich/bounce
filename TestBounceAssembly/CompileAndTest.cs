using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bounce.Framework;
using Bounce.Framework.VisualStudio;

namespace TestBounceAssembly
{
    public class CompileAndTest
    {
        [Task]
        public void Compile() {
            var vs = new VisualStudio(new Shell(Log.Default));
            var sln = vs.Solution("Bounce.sln");
            sln.Build();
        }
    }
}
