using System;
using System.Collections.Generic;
using LegacyBounce.Framework;
using VisualStudioSolution = LegacyBounce.Framework.VisualStudioSolution;

namespace Build
{
    public class Build
    {
        [Targets]
        public static object Targets(IParameters parameters)
        {
            var v4 = new VisualStudioSolution {SolutionPath = @"Bounce.sln", Configuration = "Debug"};
            var v35 = new VisualStudioSolution {SolutionPath = @"Bounce.sln", Configuration = "Debug_3_5" };

            var v4Tests = new NUnitTests
            {
                DllPaths = v4.Projects.Where(p => p.Name.EndsWith("Tests")).Select(p => p.OutputFile),
                NUnitConsolePath = @"References\NUnit\nunit-console.exe"
            };
            
            var v35Tests = new NUnitTests
            {
                DllPaths = v35.Projects.Where(p => p.Name.EndsWith("Tests")).Select(p => p.OutputFile),
                NUnitConsolePath = @"References\NUnit\nunit-console.exe"
            };

            Task<IEnumerable<string>> dests = new [] {"sdf"};
            dests.SelectTasks(dest => new Copy {ToPath = dest});

            const string nugetExe = @"References\NuGet\NuGet.exe";
            var nugetPackage = new NuGetPackage
            {
                NuGetExePath = nugetExe,
                Spec = v4.Projects["Bounce.Framework"].ProjectFile.WithDependencyOn(v4Tests, v35Tests, v35),
            };

            var nugetPush = new NuGetPush
            {
                ApiKey = EnvironmentVariables.Required<string>("NUGET_API_KEY"),
                NuGetExePath = nugetExe,
                Package = nugetPackage.Package,
            };

            return new
            {
                Net4Binaries = v4,
                Net35Binaries = v35,
                Net4Tests = v4Tests,
                Net35Tests = v35Tests,
                Binaries = new All(v4, v35),
                Tests = new All(v4Tests, v35Tests),
                NuGet = nugetPush,
                NuGetPackage = nugetPackage,
            };
        }
    }
}
