using System;
using System.Linq;
using Bounce.Framework;
using Bounce.Framework.Iis;
using Bounce.Framework.NUnit;
using Bounce.Framework.VisualStudio;

namespace Build {
    public class NewBuild {
        private readonly IVisualStudio Vs;
        private readonly INUnit NUnit;
        private readonly IIis Iis;

        public NewBuild() : this(new VisualStudio(), new NUnit(), new Iis7()) {
        }

        public NewBuild(IVisualStudio vs, INUnit nunit, IIis iis) {
            Vs = vs;
            NUnit = nunit;
            Iis = iis;
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
            NUnit.Test(sln.Projects.Where(p => p.Name.EndsWith(".Tests")).Select(p => p.OutputFile));
        }

        [Task]
        public void Deploy() {
        }
    }
}