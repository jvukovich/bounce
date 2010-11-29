using System;
using System.Collections.Generic;
using System.Linq;

namespace Bounce.Framework
{
    public class NUnitTests : Task
    {
        [Dependency] public IEnumerable<Val<string>> DllPaths;
        [Dependency] public Val<string> NUnitConsolePath;

        public NUnitTests()
        {
            NUnitConsolePath = @"c:\Program Files (x86)\NUnit\nunit-console.exe";
        }

        public override void Build(IBounce bounce)
        {
            IEnumerable<string> testDlls = DllPaths.Select(dll => dll.Value).Where(dll => dll.EndsWith("Tests.dll"));
            string joinedTestDlls = "\"" + String.Join("\" \"", testDlls.ToArray()) + "\"";

            bounce.Log.Info("running unit tests for: " + joinedTestDlls);
            bounce.ShellCommand.ExecuteAndExpectSuccess(NUnitConsolePath.Value, joinedTestDlls);
        }
    }
}