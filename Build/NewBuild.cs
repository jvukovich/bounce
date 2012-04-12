using System;
using Bounce.Framework;
using Bounce.Framework.NUnit;
using Bounce.Framework.VisualStudio;

namespace Build {
    public class NewBuild {
        [Task]
        public void CompileAndTest()
        {
            var vs = new VisualStudio();
            var sln = vs.Solution("Bounce.sln");
            sln.Build();

            var nUnit = new NUnit();
            nUnit.NUnitConsolePath = @".\References\NUnit\nunit-console.exe";
            nUnit.Test(sln.Projects["Bounce.Framework.Tests"].OutputFile);
        }
    }
}