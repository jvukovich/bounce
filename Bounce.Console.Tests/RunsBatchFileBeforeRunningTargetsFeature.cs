using System.IO;
using Bounce.Framework;
using Bounce.TestHelpers;
using Xunit;
using ProcessOutput = Bounce.Framework.ProcessOutput;

namespace Bounce.Console.Tests
{
    public class RunsBatchFileBeforeRunningTargetsFeature
    {
        // todo: dotnetcore
        private static void UnzipSolution()
        {
            FileSystemTestHelper.RecreateDirectory(@"BeforeBounceFeature");
            //new FS.FileSystem().Copy(@"..\..\BeforeBounceFeature", @"BeforeBounceFeature");
        }

        [Fact]
        public void ShouldRunBatchFileBeforeRunningTargets()
        {
            UnzipSolution();

            FileSystemTestHelper.RecreateDirectory(@"BeforeBounceFeature\bounce");

            // todo: dotnetcore
            // as there is a circular dependancy between BeforeBounceFeature.sln and the main bounce dll's
            // this needs to be run and built under the same framework version
#if (FRAMEWORKV35)
            File.WriteAllText(@"BeforeBounceFeature\bounce\beforebounce.bat", @"%SystemRoot%\Microsoft.NET\Framework\v3.5\msbuild.exe BeforeBounceFeature.sln /p:Configuration=Debug_3_5");
#else
            File.WriteAllText(@"BeforeBounceFeature\bounce\beforebounce.bat", @"%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\msbuild.exe BeforeBounceFeature.sln");
#endif

            var shell = new Shell(new FakeLog());

            ProcessOutput output = null;

            FileSystemUtils.Pushd("BeforeBounceFeature", () => output = shell.Exec(@"..\bounce.exe", "BeforeBounceFeature"));

            Assert.NotNull(output);
            Assert.Equal(0, output.ExitCode);
            Assert.Equal(string.Empty, output.Error.Trim());
            Assert.Contains("building before bounce feature", output.Output);
        }

        [Fact]
        public void ShouldPrintOutputOfScriptIfItFailed()
        {
            UnzipSolution();

            FileSystemTestHelper.RecreateDirectory(@"BeforeBounceFeature\BeforeBounceFeature\bounce");
            File.WriteAllText(@"BeforeBounceFeature\BeforeBounceFeature\bounce\beforebounce.bat", @"c:\Windows\Microsoft.NET\Framework\v3.5\msbuild.exe NonExtantSolution.sln");

            var shell = new Shell(new FakeLog());

            try
            {
                FileSystemUtils.Pushd(@"BeforeBounceFeature\BeforeBounceFeature", () => shell.Exec(@"..\..\bounce.exe", "BeforeBounceFeature"));
            }
            catch (CommandExecutionException e)
            {
                Assert.Equal(1, e.ExitCode);
                Assert.Contains("MSBUILD : error MSB1009: Project file does not exist.", e.Output);
            }
        }
    }
}