using System;
using System.Collections.Generic;
using System.Linq;

namespace Bounce.Framework {
    class LogFactoryRegistry {
        private Dictionary<string, ITaskLogFactory> LogFactories;

        public LogFactoryRegistry() {
            LogFactories = new Dictionary<string, ITaskLogFactory>();
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
        }

        public static LogFactoryRegistry Default {
            get {
                var registry = new LogFactoryRegistry();
                registry.RegisterLogFactory("default", new TaskLogFactory());
                registry.RegisterLogFactory("teamcity", new TeamCityLogFactory());
                return registry;
            }
        }
    }
}