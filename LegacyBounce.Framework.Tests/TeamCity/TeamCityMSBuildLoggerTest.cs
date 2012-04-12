using System.IO;
using Bounce.TestHelpers;
using LegacyBounce.Framework.TeamCity;
using NUnit.Framework;

namespace LegacyBounce.Framework.Tests.TeamCity {
    [TestFixture]
    public class TeamCityMSBuildLoggerTest {
        [Test]
        public void ShouldBeWarning() {
            TextWriter output = new StringWriter();

            var logger = new TeamCityMsBuildLogger("SomeSolution.sln", output, new FakeCommandLog(null, null));

            logger.CommandOutput("BounceRunner.cs(103,32): warning CS0168: The variable 'e' is declared but never used");

            Assert.That(output.ToString(), Is.EqualTo("##teamcity[message text='BounceRunner.cs(103,32): warning CS0168: The variable |'e|' is declared but never used' status='WARNING']\r\n"));
        }
    }
}