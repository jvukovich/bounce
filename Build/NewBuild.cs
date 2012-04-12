using System;
using Bounce.Framework;
using Bounce.Framework.NUnit;
using Bounce.Framework.VisualStudio;

namespace Build {
    public class NewBuild {
        private readonly IVisualStudio Vs;
        private readonly INUnit NUnit;

        public NewBuild() : this(new VisualStudio(), new NUnit()) {
        }

        public NewBuild(IVisualStudio vs, INUnit nunit) {
            Vs = vs;
            NUnit = nunit;
        }

        [Task]
        public IVisualStudioSolution Compile() {
            var sln = Vs.Solution("Bounce.sln");
            sln.Build();
            return sln;
        }

        [Task]
        public void Test() {
            var sln = Compile();
            NUnit.NUnitConsolePath = @".\References\NUnit\nunit-console.exe";
            NUnit.Test(sln.Projects["Bounce.Framework.Tests"].OutputFile);
        }
    }
}