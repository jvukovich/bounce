using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace Bounce.VisualStudio {
    public class VisualStudioCSharpProjectFileLoader : IVisualStudioProjectFileLoader {
        private XNamespace msbuild = "http://schemas.microsoft.com/developer/msbuild/2003";

        public VisualStudioCSharpProjectFileDetails LoadProject(string path, string projectName, string configuration) {
            XDocument proj = XDocument.Load(path);

            var props = new PropertyValues();
            props["Configuration"] = configuration;

            var propertyGroups = proj.Element(msbuild + "Project").Elements(msbuild + "PropertyGroup");
            LoadProperties(propertyGroups, props);

            return new VisualStudioCSharpProjectFileDetails {
                OutputFile = Path.Combine(props["OutputPath"], props["AssemblyName"] + "." + GetExtensionForOutputType(props["OutputType"])),
                Name = projectName,
            };
        }

        private string GetExtensionForOutputType(string outputType) {
            switch (outputType) {
                case "Exe":
                    return "exe";
                case "Library":
                    return "dll";
                default:
                    throw new ApplicationException("output type " + outputType + " not recognised");
            }
        }

        private void LoadProperties(IEnumerable<XElement> propertyGroups, PropertyValues props) {
            foreach (var propertyGroup in propertyGroups) {
                if (Condition(propertyGroup, props)) {
                    foreach (var propElement in propertyGroup.Elements()) {
                        if (Condition(propElement, props)) {
                            props[propElement.Name.LocalName] = propElement.Value;
                        }
                    }
                }
            }
        }

        private bool Condition(XElement element, PropertyValues props) {
            XAttribute condition = element.Attribute("Condition");
            if (condition != null) {
                var parser = new ProjectFilePropertyExpressionParser(props);
                return parser.ParseCondition(condition.Value);
            } else {
                return true;
            }
        }
    }
}