using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace Bounce.Framework.VisualStudio
{
    public class VisualStudioCSharpProjectFileLoader : IVisualStudioProjectFileLoader
    {
        private XNamespace msbuild = "http://schemas.microsoft.com/developer/msbuild/2003";

        public VisualStudioProject LoadProject(string path, string projectName, string configuration)
        {
            XDocument proj = XDocument.Load(path);

            var props = new PropertyValues();
            props["Configuration"] = configuration;

            var propertyGroups = proj.Element(msbuild + "Project").Elements(msbuild + "PropertyGroup");
            LoadProperties(propertyGroups, props);

            var projectDirectory = Path.GetDirectoryName(path);
            var outputDirectory = Path.Combine(projectDirectory, props["OutputPath"]);
            var outputFile = Path.Combine(outputDirectory, props["AssemblyName"] + "." + GetExtensionForOutputType(props["OutputType"]));

            return new VisualStudioProject
                   {
                       OutputFile = outputFile.TrimEnd('\\'),
                       OutputDirectory = outputDirectory.TrimEnd('\\'),
                       Name = projectName,
                       ProjectFile = path,
                       ProjectDirectory = projectDirectory,
                   };
        }

        private string GetExtensionForOutputType(string outputType)
        {
            switch (outputType)
            {
                case "WinExe":
                case "Exe":
                    return "exe";
                case "Library":
                    return "dll";
                default:
                    throw new ApplicationException("output type " + outputType + " not recognised");
            }
        }

        private void LoadProperties(IEnumerable<XElement> propertyGroups, PropertyValues props)
        {
            foreach (var propertyGroup in propertyGroups)
            {
                if (Condition(propertyGroup, props))
                {
                    foreach (var propElement in propertyGroup.Elements())
                    {
                        if (Condition(propElement, props))
                        {
                            props[propElement.Name.LocalName] = propElement.Value;
                        }
                    }
                }
            }
        }

        private bool Condition(XElement element, PropertyValues props)
        {
            XAttribute condition = element.Attribute("Condition");
            if (condition != null)
            {
                try
                {
                    var parser = new ProjectFilePropertyExpressionParser(props);
                    return parser.ParseCondition(condition.Value);
                } catch (ConditionParseException)
                {
                    return false;
                }
            } else
            {
                return true;
            }
        }
    }
}