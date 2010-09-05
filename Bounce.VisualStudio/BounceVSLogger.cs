using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Microsoft.Build.Framework;

namespace Bounce.VisualStudio
{
    public class BounceVSLogger : ILogger
    {
        public void Initialize(IEventSource eventSource) {
            eventSource.AnyEventRaised += eventSource_AnyEventRaised;
        }

        void eventSource_AnyEventRaised(object sender, BuildEventArgs e) {
            TargetFinishedEventArgs targf = e as TargetFinishedEventArgs;

            if (targf != null && targf.TargetName == "CopyFilesToOutputDirectory") {
                Console.WriteLine("bounce project file: " + targf.ProjectFile);
                Console.WriteLine("bounce message: " + targf.Message);
            }

            TargetStartedEventArgs targs = e as TargetStartedEventArgs;

            if (targs != null && targs.TargetName == "CopyFilesToOutputDirectory") {
                Console.WriteLine("bounce started project file: " + targs.ProjectFile);
                Console.WriteLine("bounce started target file: " + targs.TargetFile);
                Console.WriteLine("bounce started message: " + targs.Message);
            }
//            if (e is ProjectFinishedEventArgs) {
            if (e.Message.Contains("->")) {
//                Console.WriteLine("bounce: " + e.GetType().Name + e.Message);
            }
        }

        public void Shutdown() {
        }

        public LoggerVerbosity Verbosity { get; set; }

        public string Parameters { get; set; }
    }

    public class VisualStudioSolutionFileReader {
        private readonly IVisualStudioSolutionFileLoader SolutionLoader;
        private readonly IVisualStudioProjectFileLoader ProjectLoader;

        public VisualStudioSolutionFileReader()
            : this(new VisualStudioSolutionFileLoader(), new VisualStudioCSharpProjectFileLoader()) {}

        public VisualStudioSolutionFileReader(IVisualStudioSolutionFileLoader solutionLoader, IVisualStudioProjectFileLoader projectLoader) {
            SolutionLoader = solutionLoader;
            ProjectLoader = projectLoader;
        }

        public VisualStudioSolutionDetails ReadSolution(string solutionPath, string configuration) {
            VisualStudioSolutionFileDetails solutionDetails = SolutionLoader.LoadVisualStudioSolution(solutionPath);

            var projects = new List<VisualStudioCSharpProjectFileDetails>();

            foreach (var project in solutionDetails.VisualStudioProjects) {
                string projectPath = Path.Combine(Path.GetDirectoryName(solutionPath), project.Path);
                VisualStudioCSharpProjectFileDetails projectDetails = ProjectLoader.LoadProject(projectPath, configuration);
                projects.Add(projectDetails);
            }

            return new VisualStudioSolutionDetails {Projects = projects};
        }
    }

    public class VisualStudioSolutionDetails {
        public IEnumerable<VisualStudioCSharpProjectFileDetails> Projects { get; set; }
    }

    public interface IVisualStudioSolutionFileLoader {
        VisualStudioSolutionFileDetails LoadVisualStudioSolution(string path);
    }

    public class VisualStudioSolutionFileLoader : IVisualStudioSolutionFileLoader {
        public VisualStudioSolutionFileDetails LoadVisualStudioSolution(string path) {
            var csProjPattern = new Regex(@"^Project\(.*?\)\s*=\s*""(?<projname>.*?)""\s*,\s*""(?<projpath>.*?)""", RegexOptions.Multiline);

            string solutionContents = File.ReadAllText(path);
            Match match = csProjPattern.Match(solutionContents);

            var projects = new List<VisualStudioSolutionProject>();

            while (match.Success) {
                projects.Add(GetProject(match));
                match = match.NextMatch();
            }

            return new VisualStudioSolutionFileDetails { VisualStudioProjects = projects };
        }

        private VisualStudioSolutionProject GetProject(Match match) {
            return new VisualStudioSolutionProject {Name = match.Groups["projname"].Value, Path = match.Groups["projpath"].Value};
        }
    }

    public interface IVisualStudioProjectFileLoader {
        VisualStudioCSharpProjectFileDetails LoadProject(string path, string configuration);
    }

    public class VisualStudioCSharpProjectFileLoader : IVisualStudioProjectFileLoader {
        private XNamespace msbuild = "http://schemas.microsoft.com/developer/msbuild/2003";

        public VisualStudioCSharpProjectFileDetails LoadProject(string path, string configuration) {
            XDocument proj = XDocument.Load(path);

            var props = new PropertyValues();
            props["Configuration"] = configuration;

            var propertyGroups = proj.Element(msbuild + "Project").Elements(msbuild + "PropertyGroup");
            LoadProperties(propertyGroups, props);

            return new VisualStudioCSharpProjectFileDetails {
                OutputFile = Path.Combine(props["OutputPath"], props["AssemblyName"] + "." + GetExtensionForOutputType(props["OutputType"]))
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

    public class ParseResult<T> {
        public T Value;
        public int Index;
    }

    public class ProjectFilePropertyExpressionParser {
        private readonly IPropertyValues Props;


        public ProjectFilePropertyExpressionParser(IPropertyValues props) {
            Props = props;
        }

        public T Parse<T>(string sourceString, Func<char[], int, ParseResult<T>> parser) {
            char[] source = sourceString.ToCharArray();
            ParseResult<T> parseResult = parser(source, 0);

            if (parseResult != null) {
                var whitespace = ParseWhitespace(source, parseResult.Index);

                if (whitespace != null && whitespace.Index == source.Length) {
                    return parseResult.Value;
                }
            }

            throw new ConditionParseException();
        }

        private ParseResult<object> ParseWhitespace(char [] source, int index) {
            while (index < source.Length && (source[index] == ' ' || source[index] == '\t'))
            {
                index++;
            }

            return new ParseResult<object> {Index = index};
        }

        public ParseResult<bool> ParseEqualityExpression(char [] source, int index) {
            var whitespace = ParseWhitespace(source, index);
            var firstString = ParseString(source, whitespace.Index);
            if (firstString == null) {
                return null;
            }
            whitespace = ParseWhitespace(source, firstString.Index);
            var equalityOperator = ParseEquality(source, whitespace.Index);
            if (equalityOperator == null) {
                return null;
            }
            whitespace = ParseWhitespace(source, equalityOperator.Index);
            var secondString = ParseString(source, whitespace.Index);
            if (secondString == null) {
                return null;
            }

            return new ParseResult<bool> {Value = firstString.Value == secondString.Value, Index = secondString.Index};
        }

        private ParseResult<object> ParseEquality(char[] source, int index) {
            if (index >= source.Length) {
                return null;
            }
            
            if (source[index] != '=') {
                return null;
            }

            index++;
            if (index >= source.Length) {
                return null;
            }

            if (source[index] != '=') {
                return null;
            } else {
                index++;
                return new ParseResult<object> {Index = index};
            }
        }

        public ParseResult<string> ParseString(char [] source, int index) {
            if (index >= source.Length) {
                return null;
            }

            if (source[index] == '\'') {
                index++;

                var value = new StringBuilder();
                while (index < source.Length && source[index] != '\'')
                {
                    ParseResult<string> variable = ParseVariable(source, index);

                    if (variable != null) {
                        value.Append(variable.Value);
                        index = variable.Index;
                    } else {
                        value.Append(source[index]);
                    }
                    index++;
                }

                if (index < source.Length) {
                    index++;
                    return new ParseResult<string>() {Value = value.ToString(), Index = index};
                } else {
                    return null;
                }
            } else {
                return null;
            }
        }

        private ParseResult<string> ParseVariable(char[] source, int index) {
            if (source[index] != '$') {
                return null;
            }

            index++;

            if (index >= source.Length || source[index] != '(') {
                return null;
            }

            index++;

            var variable = new StringBuilder();

            while (index < source.Length && source[index] != ')') {
                variable.Append(source[index]);
                index++;
            }

            if (index < source.Length) {
                return new ParseResult<string> {Value = Props[variable.ToString()], Index = index};
            } else {
                return null;
            }
        }

        public bool ParseCondition(string value) {
            return Parse<bool>(value, ParseEqualityExpression);
        }
    }

    public class ConditionParseException : Exception {}

    public interface IPropertyValues {
        string this[string variable] { get; }
    }

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

    public class PropertyExpression {}

    public class StringPropertyExpression : PropertyExpression {
        public string Value;
    }

    public class VisualStudioCSharpProjectFileDetails {
        public string OutputFile;
    }

    public class VisualStudioSolutionFileDetails {
        public IEnumerable<VisualStudioSolutionProject> VisualStudioProjects { get; set; }
    }

    public class VisualStudioSolutionProject {
        public string Path { get; set; }
        public string Name { get; set; }
    }
}
