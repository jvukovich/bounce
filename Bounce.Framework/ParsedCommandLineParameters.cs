using System;
using System.Collections.Generic;
using System.Linq;

namespace Bounce.Framework {
    public class ParsedCommandLineParameters {
        public List<ParsedCommandLineParameter> Parameters = new List<ParsedCommandLineParameter>();
        public string[] RemainingArguments;

        public ParsedCommandLineParameter TryPopParameter(string name) {
            ParsedCommandLineParameter parameter = Parameters.FirstOrDefault(p => p.Name == name);
            if (parameter != null) {
                Parameters.Remove(parameter);
                return parameter;
            } else {
                return null;
            }
        }
    }
}