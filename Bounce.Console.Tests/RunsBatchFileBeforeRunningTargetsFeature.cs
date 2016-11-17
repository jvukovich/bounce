﻿using System.IO;
using Bounce.Framework;
using Bounce.TestHelpers;
using NUnit.Framework;
using ProcessOutput = Bounce.Framework.ProcessOutput;

namespace Bounce.Console.Tests {
    [TestFixture]
    public class RunsBatchFileBeforeRunningTargetsFeature {
        private void UnzipSolution() {
            FileSystemTestHelper.RecreateDirectory(@"BeforeBounceFeature");
            new FS.FileSystem().Copy(@"..\..\BeforeBounceFeature", @"BeforeBounceFeature");
        }

        [Test]
        public void ShouldRunBatchFileBeforeRunningTargets() {
            UnzipSolution();

            FileSystemTestHelper.RecreateDirectory(@"BeforeBounceFeature\bounce");

            //as there is a circular dependancy between BeforeBounceFeature.sln and the main bounce dll's
            //this needs to be run and built under the same framework version
#if (FRAMEWORKV35)
            File.WriteAllText(@"BeforeBounceFeature\bounce\beforebounce.bat", @"%SystemRoot%\Microsoft.NET\Framework\v3.5\msbuild.exe BeforeBounceFeature.sln /p:Configuration=Debug_3_5");
#else
            File.WriteAllText(@"BeforeBounceFeature\bounce\beforebounce.bat", @"%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\msbuild.exe BeforeBounceFeature.sln");
#endif

            var shell = new Shell(new FakeLog());

            ProcessOutput output = null;

            FileSystemUtils.Pushd(@"BeforeBounceFeature", () => output = shell.Exec(@"..\bounce.exe", "BeforeBounceFeature"));

            Assert.That(output, Is.Not.Null);
            Assert.That(output.ExitCode, Is.EqualTo(0));
            Assert.That(output.Error.Trim(), Is.EqualTo(""));
            Assert.That(output.Output, Is.StringContaining("building before bounce feature"));
        }

        [Test]
        public void ShouldPrintOutputOfScriptIfItFailed() {
            UnzipSolution();

            FileSystemTestHelper.RecreateDirectory(@"BeforeBounceFeature\BeforeBounceFeature\bounce");
            File.WriteAllText(@"BeforeBounceFeature\BeforeBounceFeature\bounce\beforebounce.bat", @"c:\Windows\Microsoft.NET\Framework\v3.5\msbuild.exe NonExtantSolution.sln");

            var shell = new Shell(new FakeLog());

            ProcessOutput output = null;

            try {
                FileSystemUtils.Pushd(@"BeforeBounceFeature\BeforeBounceFeature",
                    () => shell.Exec(@"..\..\bounce.exe", "BeforeBounceFeature"));
            } catch (CommandExecutionException e) {
                Assert.That(e.ExitCode, Is.EqualTo(1));
                Assert.That(e.Output, Is.StringContaining("MSBUILD : error MSB1009: Project file does not exist."));
            }
        }

    }
}