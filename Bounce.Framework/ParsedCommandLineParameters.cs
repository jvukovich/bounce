using System;
using System.Collections.Generic;

namespace Bounce.Framework {
    public class ParsedCommandLineParameters {
        public List<ParsedCommandLineParameter> Parameters = new List<ParsedCommandLineParameter>();
        public string[] RemainingArguments;
    }
}