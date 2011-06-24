﻿using System.Text;

namespace Bounce.Framework {
    public class TeamCityFormatter {
        private TeamCityTextFormatter TextFormatter;

        public TeamCityFormatter() {
            TextFormatter = new TeamCityTextFormatter();
        }

        public string FormatTeamCityMessageWithFields(string name, params string [] fields) {
            var output = new StringBuilder();
            output.Append("##teamcity[" + name);

            for (int i = 0; i < fields.Length - 1; i += 2) {
                output.Append(" " + fields[i] + "='" + TextFormatter.FormatTeamCityText(fields[i + 1]) + "'");
            }

            output.Append("]");

            return output.ToString();
        }

        public string FormatTeamCityMessage(string name, string message) {
            return "##teamcity[" + name + " '" + TextFormatter.FormatTeamCityText(message) + "']";
        }
    }
}