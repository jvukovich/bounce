using System;
using System.Collections.Generic;
using System.Linq;

namespace Bounce.Framework.NUnit
{
    public class NUnit : INUnit {
        /// <summary>
        /// Full path to nunit-console.exe
        /// </summary>
        public string NUnitConsolePath;

        /// <summary>
        /// Framework version to be used for tests
        /// </summary>
        public string FrameworkVersion;

        public NUnit()
        {
            NUnitConsolePath = @"c:\Program Files (x86)\NUnit\nunit-console.exe";
        }

        public void Test(string dllPath, IEnumerable<string> excludeCategories = null, IEnumerable<string> includeCategories = null) {
            Test(new[] {dllPath}, excludeCategories, includeCategories);
        }

        public void Test(IEnumerable<string> dllPaths, IEnumerable<string> excludeCategories = null, IEnumerable<string> includeCategories = null)
        {
            var joinedTestDlls = "\"" + String.Join("\" \"", dllPaths.ToArray()) + "\"";

            Bounce.Log.Info("running unit tests for: " + joinedTestDlls);

            var args = new[]
            {
                GetIncludeExcludeArgument("exclude", excludeCategories),
                GetIncludeExcludeArgument("include", includeCategories),
                Framework,
                joinedTestDlls
            };

            Bounce.Shell.ExecuteAndExpectSuccess(NUnitConsolePath, String.Join(" ", args));
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
    }
}