using Bounce.Framework.Obsolete.TeamCity;
using NUnit.Framework;

namespace Bounce.Framework.Tests.Obsolete.TeamCity {
    [TestFixture]
    public class TeamCityTextFormatterTest {
        [Test]
        public void ShouldFormatForTeamCity() {
            AsserTeamCityText("foo's test", "foo|'s test");
            AsserTeamCityText("one || two", "one |||| two");
            AsserTeamCityText("one\ntwo", "one|ntwo");
            AsserTeamCityText("one\rtwo", "one|rtwo");
            AsserTeamCityText("one]two", "one|]two");
        }

        private void AsserTeamCityText(string original, string teamcity) {
            var formatter = new TeamCityTextFormatter();
            Assert.That(formatter.FormatTeamCityText(original), Is.EqualTo(teamcity));
        }
    }
}