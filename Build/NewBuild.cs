using System;
using System.Linq;
using Bounce.Framework;
using Bounce.Framework.NUnit;
using Bounce.Framework.VisualStudio;
using Bounce.Framework.Web;

namespace Build {
    public class NewBuild {
        private readonly IVisualStudio Vs;
        private readonly INUnit NUnit;
        private readonly IWebServer WebServer;

        public NewBuild(IShell shell, ILog log)
            : this(new VisualStudio(shell), new NUnit(shell, log), new Iis7()) {}

        public NewBuild()
            : this(new Log(LogLevel.Info)) {}

        public NewBuild(ILog log) : this(new Shell(log), log) {
        }

        public NewBuild(IVisualStudio vs, INUnit nunit, IWebServer webServer) {
            Vs = vs;
            NUnit = nunit;
            NUnit.NUnitConsolePath = @".\References\NUnit\nunit-console.exe";
            WebServer = webServer;
        }

        [Task]
        public IVisualStudioSolution Compile() {
            var sln = Solution;
            sln.Build();
            return sln;
        }

        private IVisualStudioSolution Solution {
            get { return Vs.Solution("Bounce.sln"); }
        }

        [Task]
        public void Test(string something = "sdflksdf") {
            RunTests(Compile(), "Tests");
        }

        [Task]
        public void IntegrationTest() {
            var sln = Compile();
            RunTests(sln, "IntegrationTests");
            RunTests(sln, "Tests");
        }

        private void RunTests(IVisualStudioSolution sln, string testSuffix) {
            NUnit.Test(sln.Projects.Where(p => p.Name.EndsWith("."+ testSuffix)).Select(p => p.OutputFile));
        }

        [Task]
        public void Deploy() {
            var site = WebServer.WebSite("MyWebSite");
            site.Directory = Compile().Projects["Bounce.Framework"].ProjectDirectory;
            site.Bindings.Add("http://*:4000/");
            site.Save();
        }
    }
}