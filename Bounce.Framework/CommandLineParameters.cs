using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Bounce.Framework {
    public class CommandLineParameters : ICommandLineParameters {
        private ITypeParsers TypeParsers;
        private Dictionary<string, IParameter> RegisteredParameters;

        public CommandLineParameters(ITypeParsers typeParsers) {
            TypeParsers = typeParsers;
            RegisteredParameters = new Dictionary<string, IParameter>();
        }

        public CommandLineParameters() : this(Framework.TypeParsers.Default) {
        }

        public IEnumerable<IParameter> Parameters {
            get { return RegisteredParameters.Values; }
        }

        private Parameter<T> RegisterParameter<T>(Parameter<T> p) {
            if (RegisteredParameters.ContainsKey(p.Name)) {
                throw new BounceException(String.Format("parameter `{0}' already registered", p.Name));
            }

            RegisteredParameters.Add(p.Name, p);
            return p;
        }

        public Future<T> Required<T>(string name) {
            return RegisterParameter(new Parameter<T> {Name = name, Required = true});
        }

        public Future<T> Default<T>(string name, T defaultValue) {
            return RegisterParameter(new Parameter<T> {Name = name, DefaultValue = defaultValue});
        }

        public Future<T> OneOf<T>(string name, IEnumerable<T> availableValues) {
            return RegisterParameter(new Parameter<T> { Name = name, Required = true, AvailableValues = availableValues.Cast<object>() });
        }

        public Future<T> OneOfWithDefault<T>(string name, T defaultValue, IEnumerable<T> availableValues) {
            return RegisterParameter(new Parameter<T> { Name = name, DefaultValue = defaultValue, AvailableValues = availableValues.Cast<object>() });
        }

        public void RegisterTypeParser<T>(ITypeParser parser) {
            TypeParsers.RegisterTypeParser<T>(parser);
        }

        private void EnsureThatRequiredParametersAreSet(ParameterErrors parameterErrors) {
        }

        public void ParseCommandLineArguments(List<ParsedCommandLineParameter> parameters) {
            WithRiskOfParameterErrors(parameterErrors => {
                foreach (var commandLineParameter in parameters) {
                    IParameter parameter;
                    if (RegisteredParameters.TryGetValue(commandLineParameter.Name, out parameter)) {
                        parameter.Parse(commandLineParameter.Value, TypeParsers);
                    } else {
                        parameterErrors.NoSuchParameter(commandLineParameter.Name);
                    }
                }
            });
        }

        private static void WithRiskOfParameterErrors(Action<ParameterErrors> action) {
            var parameterErrors = new ParameterErrors();

            action(parameterErrors);

            parameterErrors.ThrowIfThereAreErrors();
        }

        public void EnsureAllRequiredParametersHaveValues(IEnumerable<IParameter> parameters) {
            WithRiskOfParameterErrors(parameterErrors => {
                foreach (var required in parameters.Where(p => p.Required && !p.HasValue)) {
                    if (required != null) {
                        parameterErrors.RequiredParameterNotSet(required.Name);
                    }
                }
            });
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
            if (Errors.Count > 0) {
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