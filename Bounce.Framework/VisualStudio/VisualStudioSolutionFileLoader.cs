using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Bounce.Framework.VisualStudio
{
    public class VisualStudioSolutionFileLoader : IVisualStudioSolutionFileLoader
    {
        public VisualStudioSolutionFileDetails LoadVisualStudioSolution(string path)
        {
            var csProjPattern = new Regex(@"^Project\(.*?\)\s*=\s*""(?<projname>.*?)""\s*,\s*""(?<projpath>.*?)""", RegexOptions.Multiline);

            string solutionContents = File.ReadAllText(path);
            Match match = csProjPattern.Match(solutionContents);

            var projects = new List<VisualStudioSolutionProjectReference>();

            while (match.Success)
            {
                projects.Add(GetProject(match, path));
                match = match.NextMatch();
            }

            return new VisualStudioSolutionFileDetails {VisualStudioProjects = projects};
        }

        private VisualStudioSolutionProjectReference GetProject(Match match, string solutionPath)
        {
            return new VisualStudioSolutionProjectReference {Name = match.Groups["projname"].Value, Path = Path.Combine(Path.GetDirectoryName(solutionPath), match.Groups["projpath"].Value)};
        }
    }
}