using System.IO;
using Bounce.Framework.Obsolete.TeamCity;
using Bounce.TestHelpers;
using NUnit.Framework;

namespace Bounce.Framework.Tests.Obsolete.TeamCity
{
    [TestFixture]
    public class TeamCity5NUnitLoggerTests {
        [Test]
        public void OutputsTeamCityLogMessageWithInconclusiveTests() {
            TextWriter output = new StringWriter();
            var sut = new TeamCity5NUnitLogger(string.Empty, output, new FakeCommandLog(null, null));
            sut.CommandOutput(@"Tests run: 41, Errors: 4, Failures: 0, Inconclusive: 9, Time: 37 seconds");

            const string expected = "##teamcity[buildStatus text='{build.status.text}, inconclusive: 9']\r\n";
            Assert.That(output.ToString(), Is.EqualTo(expected));
        }
    }
}