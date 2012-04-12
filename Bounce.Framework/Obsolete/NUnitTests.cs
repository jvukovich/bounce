using System;
using System.Collections.Generic;
using System.Linq;

namespace Bounce.Framework.Obsolete
{
    public class NUnitTests : Task
    {
        /// <summary>
        /// List of paths to test DLLs
        /// </summary>
        [Dependency] public Task<IEnumerable<string>> DllPaths;

        /// <summary>
        /// Full path to nunit-console.exe
        /// </summary>
        [Dependency] public Task<string> NUnitConsolePath;

        /// <summary>
        /// Categories to include in the test run.
        /// </summary>
        [Dependency] public Task<IEnumerable<string>> IncludeCategories;

        /// <summary>
        /// Framework version to be used for tests
        /// </summary>
        [Dependency]
        public Task<string> FrameworkVersion;

        /// <summary>
        /// Categories to exclude in the test run.
        /// </summary>
        [Dependency] public Task<IEnumerable<string>> ExcludeCategories;

        public NUnitTests()
        {
            NUnitConsolePath = @"c:\Program Files (x86)\NUnit\nunit-console.exe";
            ExcludeCategories = new string[0];
            IncludeCategories = new string[0];
        }

        public override void Build(IBounce bounce)
        {
            var joinedTestDlls = "\"" + String.Join("\" \"", DllPaths.Value.ToArray()) + "\"";

            bounce.Log.Info("running unit tests for: " + joinedTestDlls);

            var args = new[]
            {
                Excludes,
                Includes,
                Framework,
                joinedTestDlls
            };

            bounce.ShellCommand.ExecuteAndExpectSuccess(NUnitConsolePath.Value, String.Join(" ", args));
        }

        protected string Excludes
        {
            get
            {
                return GetIncludeExcludeArgument("exclude", ExcludeCategories);
            }
        }

        protected string Includes
        {
            get {
                return GetIncludeExcludeArgument("include", IncludeCategories);
            }
        }

        protected string Framework {
            get {
                if (FrameworkVersion != null && !string.IsNullOrEmpty(FrameworkVersion.Value)) {
                    return "/framework=" + FrameworkVersion.Value;
                }

                return "";
            }
        }

        private static string GetIncludeExcludeArgument(string argumentName, Task<IEnumerable<string>> categories)
        {
            if (categories.Value.Count() > 0)
            {
                return "/" + argumentName + "=" + String.Join(",", categories.Value.ToArray());
            }
            return "";
        }
    }
}