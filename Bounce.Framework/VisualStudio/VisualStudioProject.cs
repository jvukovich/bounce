namespace Bounce.Framework.VisualStudio
{
    public class VisualStudioProject : MsBuildFile, IVisualStudioProject
    {
        public string OutputFile { get; set; }
        public string Name { get; set; }
        public string OutputDirectory { get; set; }
        public string ProjectFile { get; set; }
        public string ProjectDirectory { get; set; }
        public VisualStudioSolution Solution { get; set; }

        protected override string Path
        {
            get { return ProjectFile; }
        }

        public override IMsBuild MsBuild
        {
            get { return Solution.MsBuild; }
            set { Solution.MsBuild = value; }
        }
    }
}