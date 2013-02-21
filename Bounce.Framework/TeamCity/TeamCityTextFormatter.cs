namespace Bounce.Framework.TeamCity {
    public class TeamCityTextFormatter {
        public string FormatTeamCityText(string text) {
            return text.Replace("|", "||").Replace("'", "|'").Replace("\n", "|n").Replace("\r", "|r").Replace("]", "|]");
        }
    }
}