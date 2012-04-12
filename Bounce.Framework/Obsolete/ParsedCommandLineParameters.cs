using System;
using System.Collections.Generic;
using System.Linq;

namespace Bounce.Framework.Obsolete {
    public class ParsedCommandLineParameters {
        public List<ParsedCommandLineParameter> Parameters = new List<ParsedCommandLineParameter>();
        public string[] RemainingArguments;

        public void IfParameterDo(string name, Action<string> withParameter) {
            ParsedCommandLineParameter parameter = Parameters.FirstOrDefault(p => p.Name == name);
            if (parameter != null) {
                Parameters.Remove(parameter);
                withParameter(parameter.Value);
            }
        }
    }
}