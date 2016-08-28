using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Bounce.Framework.VisualStudio {
    class MsBuildPathDiscovery {

        public Func<string, bool> FileExitsCheck { get; set; }
        public readonly List<string> MsBuildLocations;

        public MsBuildPathDiscovery(Func<string, bool> fileExitsCheck = null) {
            FileExitsCheck = fileExitsCheck ?? File.Exists;

            MsBuildLocations = new List<string> {
                Environment.ExpandEnvironmentVariables(@"%MSBuild%"),
                Environment.ExpandEnvironmentVariables(@"%ProgramFiles%\MSBuild\14.0\Bin\msbuild.exe"),
                Environment.ExpandEnvironmentVariables(@"%ProgramFiles%\MSBuild\12.0\Bin\msbuild.exe"),
                Environment.ExpandEnvironmentVariables(@"%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\msbuild.exe")
            };

            if (MsBuildLocations.First() == "%MSBuild%") {
                MsBuildLocations.RemoveAt(0);
            }
        }

        public string LocateMostRecentMsBuildPath() {
            foreach (var location in MsBuildLocations) {
                if (FileExitsCheck(location)) {
                    return location;
                }
            }
            return MsBuildLocations.Last();
        }
    }
}