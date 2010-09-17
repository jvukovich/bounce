using System;
using System.Collections.Generic;
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

        private IParameter<T> RegisterParameter<T>(IParameter<T> p) {
            RegisteredParameters.Add(p.Name, p);
            return p;
        }

        public IValue<T> Required<T>(string name) {
            return RegisterParameter(new Parameter<T> {Name = name, Required = true});
        }

        public IValue<T> Default<T>(string name, T defaultValue) {
            return RegisterParameter(new Parameter<T> {Name = name, Value = defaultValue});
        }

        public IValue<T> OneOf<T>(string name, IEnumerable<T> availableValues) {
            return RegisterParameter(new Parameter<T> { Name = name, Required = true, AvailableValues = availableValues.Cast<object>() });
        }

        public IValue<T> OneOfWithDefault<T>(string name, T defaultValue, IEnumerable<T> availableValues) {
            return RegisterParameter(new Parameter<T> { Name = name, Value = defaultValue, AvailableValues = availableValues.Cast<object>() });
        }

        public void RegisterTypeParser<T>(Func<string, T> parser) {
            TypeParsers.Add(typeof (T), s => parser(s));
        }

        public string [] ParseCommandLineArguments(string [] args) {
            var remainingArgs = new List<string>();

            for (int n = 0; n < args.Length; n++) {
                string arg = args[n];

                if (arg.StartsWith("/")) {
                    string argName = arg.Substring(1);
                    if ((n + 1) < args.Length) {
                        string argValue = args[++n];
                        RegisteredParameters[argName].Parse(argValue, TypeParsers);
                    } else {
                        throw new ConfigurationException("expected value for argument " + argName);
                    }
                } else {
                    remainingArgs.Add(arg);
                }
            }

            var firstRequiredWithoutValue = RegisteredParameters.Select(p => p.Value).Where(p => p.Required && !p.HasValue).FirstOrDefault();
            if (firstRequiredWithoutValue != null) {
                throw new ConfigurationException("expected value for argument " + firstRequiredWithoutValue.Name);
            }

            return remainingArgs.ToArray();
        }
    }

    public class ConfigurationException : Exception {
        public ConfigurationException(string message) : base(message) {}
    }
}