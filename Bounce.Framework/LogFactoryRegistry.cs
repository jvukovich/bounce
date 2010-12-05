using System;
using System.Collections.Generic;
using System.Linq;

namespace Bounce.Framework {
    public class LogFactoryRegistry {
        private Dictionary<string, ITaskLogFactory> LogFactories;
        private Dictionary<ITaskLogFactory, string> LogFactoryNames;

        public LogFactoryRegistry() {
            LogFactories = new Dictionary<string, ITaskLogFactory>();
            LogFactoryNames = new Dictionary<ITaskLogFactory, string>();
        }

        public ITaskLogFactory GetLogFactoryByName(string name) {
            ITaskLogFactory logFactory;
            if (LogFactories.TryGetValue(name, out logFactory)) {
                return logFactory;
            } else {
                throw new ConfigurationException(String.Format("no such logformat named {0}. Try one of: {1}", name, String.Join(", ", LogFactories.Keys.ToArray())));
            }
        }

        public void RegisterLogFactory(string name, ITaskLogFactory logFactory) {
            LogFactories.Add(name, logFactory);
            LogFactoryNames.Add(logFactory, name);
        }

        public static LogFactoryRegistry Default {
            get {
                var registry = new LogFactoryRegistry();
                registry.RegisterLogFactory("default", new TaskLogFactory());
                registry.RegisterLogFactory("teamcity", new TeamCityLogFactory());
                return registry;
            }
        }

        public string FindNameForLogFactory(ITaskLogFactory logFactory) {
            string name;
            if (LogFactoryNames.TryGetValue(logFactory, out name)) {
                return name;
            } else {
                return null;
            }
        }
    }
}