using LegacyBounce.Framework.TeamCity;
using NUnit.Framework;

namespace LegacyBounce.Framework.Tests.TeamCity
{
    [TestFixture]
    public class TeamCityLogTests
    {
        [Test]
        public void ShouldReturnTeamCityMsBuildLoggerForMsBuildExe()
        {
            var sut = new TeamCityLog(null, new LogOptions(), null);
            var logger = sut.BeginExecutingCommand("msbuild.exe", @"c:\temp\mysolution.sln");

            Assert.That(logger, Is.InstanceOf<TeamCityMsBuildLogger>());
        }

        [Test]
        public void ShouldReturnTeamCityNUnitLoggerForNUnitConsole()
        {
            var sut = new TeamCityLog(null, new LogOptions(), null);
            var logger = sut.BeginExecutingCommand("NUnit-Console.exe", @"c:\temp\mysolution\bin\mysolution.tests.dll /noshadow");

            Assert.That(logger, Is.InstanceOf<TeamCityNUnitLogger>());
        }

        [Test]
        public void ShouldReturnTeamCityNUnitLoggerForNUnitConsoleX86()
        {
            var sut = new TeamCityLog(null, new LogOptions(), null);
            var logger = sut.BeginExecutingCommand("nunit-console-x86.exe", @"c:\temp\mysolution\bin\mysolution.tests.dll /noshadow");

            Assert.That(logger, Is.InstanceOf<TeamCityNUnitLogger>());
        }

        [Test]
        public void ShouldReturnTeamCityNUnitLoggerForNUnitConsoleRunningUnderPartCover()
        {
            var sut = new TeamCityLog(null, new LogOptions(), null);
            var logger = sut.BeginExecutingCommand("PartCover.exe", "--register --output \"PartCover.CoverTests.xml\" --target \"deploy\\nunit-console.exe\" --target-args \"\\\"UnitTests.dll\\\" /noshadow\" --include [*]* --exclude [log4net*]*");

            Assert.That(logger, Is.InstanceOf<TeamCityNUnitLogger>());
        }
    }
}