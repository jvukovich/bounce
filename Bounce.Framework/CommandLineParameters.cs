using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Bounce.Framework {
    public class CommandLineParameters : IParameters {
        private TypeParsers TypeParsers;
        private Dictionary<string, IParameter> RegisteredParameters;

        public CommandLineParameters() {
            TypeParsers = new TypeParsers();
            RegisteredParameters = new Dictionary<string, IParameter>();
        }

        public IEnumerable<IParameter> Parameters {
            get { return RegisteredParameters.Values; }
        }

        public static CommandLineParameters ParametersWithUsualTypeParsers() {
            var p = new CommandLineParameters();
            p.RegisterTypeParser(s => int.Parse(s));
            p.RegisterTypeParser(s => s);
            p.RegisterTypeParser(s => DateTime.Parse(s));
            return p;
        }

        private Parameter<T> RegisterParameter<T>(Parameter<T> p) {
            if (RegisteredParameters.ContainsKey(p.Name)) {
                throw new BounceException(String.Format("parameter `{0}' already registered", p.Name));
            }

            RegisteredParameters.Add(p.Name, p);
            return p;
        }

        public Val<T> Required<T>(string name) {
            return RegisterParameter(new Parameter<T> {Name = name, Required = true});
        }

        public Val<T> Default<T>(string name, T defaultValue) {
            return RegisterParameter(new Parameter<T> {Name = name, DefaultValue = defaultValue});
        }

        public Val<T> OneOf<T>(string name, IEnumerable<T> availableValues) {
            return RegisterParameter(new Parameter<T> { Name = name, Required = true, AvailableValues = availableValues.Cast<object>() });
        }

        public Val<T> OneOfWithDefault<T>(string name, T defaultValue, IEnumerable<T> availableValues) {
            return RegisterParameter(new Parameter<T> { Name = name, DefaultValue = defaultValue, AvailableValues = availableValues.Cast<object>() });
        }

        public void RegisterTypeParser<T>(Func<string, T> parser) {
            TypeParsers.Add(typeof (T), s => parser(s));
        }

        private void EnsureThatRequiredParametersAreSet(ParameterErrors parameterErrors) {
            foreach (var required in RegisteredParameters.Select(p => p.Value).Where(p => p.Required && !p.HasValue)) {
                if (required != null) {
                    parameterErrors.RequiredParameterNotSet(required.Name);
                }
            }
        }

        public void ParseCommandLineArguments(List<ParsedCommandLineParameter> parameters) {
            var parameterErrors = new ParameterErrors();

            foreach (var commandLineParameter in parameters) {
                IParameter parameter;
                if (RegisteredParameters.TryGetValue(commandLineParameter.Name, out parameter)) {
                    parameter.Parse(commandLineParameter.Value, TypeParsers);
                } else {
                    parameterErrors.NoSuchParameter(commandLineParameter.Name);
                }
            }

            EnsureThatRequiredParametersAreSet(parameterErrors);

            parameterErrors.ThrowIfThereAreErrors();
        }
    }

    public abstract class ParameterError {
        public string Name;

        public abstract void Explain(TextWriter stderr);
    }

    internal class NoSuchParameter : ParameterError {
        public override void Explain(TextWriter stderr) {
            stderr.WriteLine("no such parameter {0}", Name);
        }
    }

    internal class RequiredParameterNotSet : ParameterError {
        public override void Explain(TextWriter stderr) {
            stderr.WriteLine("required parameter {0} not given value", Name);
        }
    }

    public class ParameterErrors {
        List<ParameterError> Errors = new List<ParameterError>();

        public void NoSuchParameter(string name) {
            Errors.Add(new NoSuchParameter {Name = name});
        }

        public void RequiredParameterNotSet(string name) {
            Errors.Add(new RequiredParameterNotSet {Name = name});
        }

        public void ThrowIfThereAreErrors() {
            if (Errors.Count > 1) {
                throw new CommandLineParametersException(Errors);
            }
        }
    }

    public class CommandLineParametersException : BounceException {
        private readonly List<ParameterError> Errors;

        public CommandLineParametersException(List<ParameterError> errors) : base("command line argument errors") {
            Errors = errors;
        }

        public override void Explain(TextWriter stderr) {
            foreach (var error in Errors) {
                error.Explain(stderr);
            }
        }
    }

    public class ConfigurationException : BounceException {
        public ConfigurationException(string message) : base(message) {}
    }
}