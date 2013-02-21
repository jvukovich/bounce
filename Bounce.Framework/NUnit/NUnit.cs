using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Bounce.Framework.TeamCity;

namespace Bounce.Framework.NUnit
{
    public class NUnit : INUnit {
        private readonly IShell Shell;
        private readonly ILog Log;

        /// <summary>
        /// Full path to nunit-console.exe
        /// </summary>
        public string NUnitConsolePath { get; set; }

        /// <summary>
        /// Framework version to be used for tests
        /// </summary>
        public string FrameworkVersion { get; set; }
        public string TeamCityNUnitVersion { get; set; }
        public string TeamCityPlatform { get; set; }

        public NUnit(IShell shell, ILog log) {
            Shell = shell;
            Log = log;
            NUnitConsolePath = @"nunit-console.exe";
            FrameworkVersion = Environment.Version.ToString(2);
            TeamCityPlatform = "MSIL";
            TeamCityNUnitVersion = "NUnit-2.6.2";
        }

        public void Test(string dllPath, IEnumerable<string> excludeCategories = null, IEnumerable<string> includeCategories = null) {
            Test(new[] {dllPath}, excludeCategories, includeCategories);
        }

        public void Test(IEnumerable<string> dllPaths, IEnumerable<string> excludeCategories = null, IEnumerable<string> includeCategories = null)
        {
            var joinedTestDlls = "\"" + String.Join("\" \"", dllPaths.ToArray()) + "\"";

            Log.Info("running unit tests for: " + joinedTestDlls);


            if (TeamCityFormatter.IsActive) {
                RunNUnitWithTeamCity(joinedTestDlls, excludeCategories, includeCategories);
            } else {
                RunNUnit(joinedTestDlls, excludeCategories, includeCategories);
            }
        }

        private void RunNUnitWithTeamCity(string joinedTestDlls, IEnumerable<string> excludeCategories, IEnumerable<string> includeCategories) {
            var args = new[] {
                TeamCityFrameworkVersion,
                TeamCityPlatform,
                TeamCityNUnitVersion,
                GetTeamCityIncludeExcludeArgument("category-exclude", excludeCategories),
                GetTeamCityIncludeExcludeArgument("category-include", includeCategories),
                joinedTestDlls
            };

            var nunitLauncher = Environment.GetEnvironmentVariable("teamcity.dotnet.nunitlauncher");
            Shell.Exec(nunitLauncher, String.Join(" ", args));
        }

        private void RunNUnit(string joinedTestDlls, IEnumerable<string> excludeCategories, IEnumerable<string> includeCategories) {
            var args = new[] {
                GetIncludeExcludeArgument("exclude", excludeCategories),
                GetIncludeExcludeArgument("include", includeCategories),
                Framework,
                joinedTestDlls
            };

            Shell.Exec(NUnitConsolePath, string.Join(" ", args));
        }

        protected string TeamCityFrameworkVersion {
            get { return "v" + FrameworkVersion; }
        }

        protected string Framework {
            get {
                if (!string.IsNullOrEmpty(FrameworkVersion)) {
                    return "/framework=" + FrameworkVersion;
                }

                return "";
            }
        }

        private static string GetIncludeExcludeArgument(string argumentName, IEnumerable<string> categories)
        {
            if (categories != null && categories.Any())
            {
                return "/" + argumentName + "=" + String.Join(",", categories.ToArray());
            }
            return "";
        }

        private static string GetTeamCityIncludeExcludeArgument(string argumentName, IEnumerable<string> categories)
        {
            if (categories != null && categories.Any())
            {
                return "/" + argumentName + ":" + String.Join(";", categories.ToArray());
            }
            return "";
        }
    }
}