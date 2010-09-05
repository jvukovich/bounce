using System.Collections.Generic;

namespace Bounce.VisualStudio {
    public class PropertyValues : IPropertyValues {
        private Dictionary<string, string> Properties;

        public PropertyValues () {
            Properties = new Dictionary<string, string>();
        }

        public string this[string variable] {
            get {
                string value;
                if (Properties.TryGetValue(variable, out value)) {
                    return value;
                } else {
                    return "";
                }
            }
            set { Properties[variable] = value; }
        }
    }
}