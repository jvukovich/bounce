using Bounce.Framework.Obsolete;
using Moq;
using NUnit.Framework;

namespace Bounce.Framework.Tests.Obsolete
{
    [TestFixture]
    public class NUnitTestsWithPartCoverTests
    {
        const string PartCoverPath = @"C:\Program Files (x86)\PartCover\PartCover .NET 4.0\partcover.exe";
        const string NunitConsolePath = @"c:\Program Files (x86)\NUnit 2.5.10\bin\net-2.0\nunit-console-x86.exe";
        const string Dll1 = @"UnitTests.dll";
        const string Dll2 = @"IntegrationTests.dll";

        [Test]
        public void ShouldPassCorrectCommandLineArguments()
        {
            var mockBounce = new Mock<IBounce>();
            var mockShellCommand = new Mock<IShellCommandExecutor>();
            mockBounce.Setup(b => b.ShellCommand).Returns(mockShellCommand.Object);

            var sut = new NUnitTestsWithPartCover
            {
                PartCoverPath = PartCoverPath,
                NUnitConsolePath = NunitConsolePath,
                DllPaths = new[] { Dll1, Dll2 },
                IncludeRules = new[] { "[*]*" },
                ExcludeRules = new[] { "[Iesi.Collections]*", "[Microsoft*]*" },
                RegisterPartCoverDlls = false
            };

            sut.Build(mockBounce.Object);

            const string expectedArgs = "--output \"partcover.xml\" " +
                                        "--target \"" + NunitConsolePath + "\" " +
                                        "--target-args \"\\\"" + Dll1 + "\\\" \\\"" + Dll2 + "\\\" /noshadow\" " +
                                        "--include [*]* " +
                                        "--exclude [Iesi.Collections]* " +
                                        "--exclude [Microsoft*]*";

            
            mockShellCommand.Verify(msc => msc.ExecuteAndExpectSuccess(PartCoverPath, expectedArgs));
        }

        [Test]
        public void ShouldPassNunitIncludesAndExcludes()
        {
            var mockBounce = new Mock<IBounce>();
            var mockShellCommand = new Mock<IShellCommandExecutor>();
            mockBounce.Setup(b => b.ShellCommand).Returns(mockShellCommand.Object);

            var sut = new NUnitTestsWithPartCover
            {
                PartCoverPath = PartCoverPath,
                NUnitConsolePath = NunitConsolePath,
                DllPaths = new[] { Dll1, Dll2 },
                IncludeCategories = new [] {"UnitTest", "IntegrationTest"},
                ExcludeCategories = new [] {"Slow"},
                RegisterPartCoverDlls = false
            };

            sut.Build(mockBounce.Object);

            const string expectedArgs = "--output \"partcover.xml\" " +
                                        "--target \"" + NunitConsolePath + "\" " +
                                        "--target-args \"\\\"" + Dll1 + "\\\" \\\"" + Dll2 + "\\\" /include=UnitTest,IntegrationTest /exclude=Slow /noshadow\"";

            mockShellCommand.Verify(msc => msc.ExecuteAndExpectSuccess(PartCoverPath, expectedArgs));
        }

        [Test]
        public void ShouldPassRegisterFlag()
        {
            var mockBounce = new Mock<IBounce>();
            var mockShellCommand = new Mock<IShellCommandExecutor>();
            mockBounce.Setup(b => b.ShellCommand).Returns(mockShellCommand.Object);

            var sut = new NUnitTestsWithPartCover
            {
                PartCoverPath = PartCoverPath,
                NUnitConsolePath = NunitConsolePath,
                DllPaths = new[] { Dll1, Dll2 },
                RegisterPartCoverDlls = true
            };

            sut.Build(mockBounce.Object);

            const string expectedArgs = "--register " +
                                        "--output \"partcover.xml\" " +
                                        "--target \"" + NunitConsolePath + "\" " +
                                        "--target-args \"\\\"" + Dll1 + "\\\" \\\"" + Dll2 + "\\\" /noshadow\"";

            mockShellCommand.Verify(msc => msc.ExecuteAndExpectSuccess(PartCoverPath, expectedArgs));
        }

        [Test]
        public void ShouldNotPassNoShadowFlagToNUnit()
        {
            var mockBounce = new Mock<IBounce>();
            var mockShellCommand = new Mock<IShellCommandExecutor>();
            mockBounce.Setup(b => b.ShellCommand).Returns(mockShellCommand.Object);

            var sut = new NUnitTestsWithPartCover
            {
                PartCoverPath = PartCoverPath,
                NUnitConsolePath = NunitConsolePath,
                DllPaths = new[] { Dll1, Dll2 },
                ShadowCopyNUnitFiles = true
            };

            sut.Build(mockBounce.Object);

            const string expectedArgs = "--register " +
                                        "--output \"partcover.xml\" " +
                                        "--target \"" + NunitConsolePath + "\" " +
                                        "--target-args \"\\\"" + Dll1 + "\\\" \\\"" + Dll2 + "\\\"\"";

            mockShellCommand.Verify(msc => msc.ExecuteAndExpectSuccess(PartCoverPath, expectedArgs));
        }
    }
}