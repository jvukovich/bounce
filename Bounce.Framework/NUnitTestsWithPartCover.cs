using System;
using System.Collections.Generic;
using System.Linq;

namespace Bounce.Framework
{
    public class NUnitTestsWithPartCover : NUnitTests
    {
        /// <summary>
        /// Full path to partcover.exe
        /// </summary>
        [Dependency] public Task<string> PartCoverPath;

        /// <summary>
        /// Include namespace rules to pass to partcover.
        /// eg: "[MyNamespace.MyAssembly]*"
        /// Will be output as: "--include=[MyNamespace.MyAssembly]*"
        /// </summary>
        [Dependency] public Task<IEnumerable<string>> IncludeRules;

        /// <summary>
        /// Exclude namespace rules to pass to partcover.
        /// eg: "[MyNamespace.MyAssembly]*"
        /// Will be output as: "--exclude=[MyNamespace.MyAssembly]*"
        /// </summary>
        [Dependency] public Task<IEnumerable<string>> ExcludeRules;

        /// <summary>
        /// Full path to partcover output xml file
        /// </summary>
        [Dependency] public Task<string> OutputPath;

        public NUnitTestsWithPartCover()
        {
            OutputPath = "partcover.xml";
            PartCoverPath = @"C:\Program Files (x86)\PartCover\PartCover .NET 4.0\PartCover.exe";
            ExcludeRules = new string[0];
            IncludeRules = new string[0];
        }

        public override void Build(IBounce bounce)
        {
            var args = new[]
            {
                Output,
                Target,
                TargetArgs,
                IncludedRules,
                ExcludedRules
            }.Where(arg => arg != "").ToArray();

            bounce.ShellCommand.ExecuteAndExpectSuccess(PartCoverPath.Value, String.Join(" ", args));
        }

        private string Output
        {
            get { return "--output " + "\"" + OutputPath.Value + "\""; }
        }

        private string Target
        {
            get { return "--target " + "\"" + NUnitConsolePath.Value + "\""; }
        }

        private string TargetArgs
        {
            get { return string.Format("--target-args \"{0}\"", NUnitArguments); }
        }

        private string NUnitArguments
        {
            get
            {
                var joinedTestDlls = "\\\"" + String.Join("\\\" \\\"", DllPaths.Value.ToArray()) + "\\\"";
                var args = new[]
                {
                    joinedTestDlls,
                    Includes,
                    Excludes,
                    "/noshadow"
                }.Where(arg => arg != "").ToArray();

                return String.Join(" ", args).Trim();
            }
        }

        private string IncludedRules
        {
            get { return String.Join(" ", GetIncludeExcludeArgument("include", IncludeRules.Value).ToArray()); }
        }

        private string ExcludedRules
        {
            get { return String.Join(" ", GetIncludeExcludeArgument("exclude", ExcludeRules.Value).ToArray()); }
        }

        private static IEnumerable<string> GetIncludeExcludeArgument(string argumentName, IEnumerable<string> rules)
        {
            return rules.Select(rule => string.Format("--{0} {1}", argumentName, rule));
        }
    }
}