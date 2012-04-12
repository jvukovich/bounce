using Bounce.Framework;
using Bounce.Framework.VisualStudio;

namespace Build {
    public class NewBuild {
        [Task]
        public void Compile()
        {
            var vs = new VisualStudio();
            var sln = vs.Solution("Bounce.sln");
            sln.Build();
        }
    }
}