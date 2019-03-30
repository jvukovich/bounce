using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Bounce.Framework.VisualStudio
{
    public class VisualStudioSolutionFileReader
    {
        private readonly IVisualStudioSolutionFileLoader SolutionLoader;

        public VisualStudioSolutionFileReader()
            : this(new VisualStudioSolutionFileLoader())
        {
        }

        public VisualStudioSolutionFileReader(IVisualStudioSolutionFileLoader solutionLoader)
        {
            SolutionLoader = solutionLoader;
        }

        public VisualStudioSolution ReadSolution(string solutionPath, string configuration)
        {
            VisualStudioSolutionFileDetails solutionDetails = SolutionLoader.LoadVisualStudioSolution(solutionPath);

            var csProjectReferences = solutionDetails.VisualStudioProjects.Where(r => Path.GetExtension(r.Path) == ".csproj");
            var sln = new VisualStudioSolution(solutionPath, configuration, csProjectReferences);

            return sln;
        }
    }
}