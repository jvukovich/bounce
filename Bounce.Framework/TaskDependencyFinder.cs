using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Bounce.Framework {
    public class TaskDependencyFinder {
        static TaskDependencyFinder() {
            Instance = new TaskDependencyFinder();
        }

        public static TaskDependencyFinder Instance { get; private set; }

        private class TypeDependencyGetter {
            private IEnumerable<FieldInfo> Fields;
            private IEnumerable<PropertyInfo> Properties;

            public TypeDependencyGetter(Type type) {
                var bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

                Fields = type
                    .GetFields(bindingFlags)
                    .Where(IsDependency)
                    .ToArray();

                Properties = type
                    .GetProperties(bindingFlags)
                    .Where(IsDependency)
                    .ToArray();
            }

            private class DependencyNameValue {
                public DependencyNameValue(string name, object value) {
                    Name = name;
                    Value = value;
                }

                public string Name;
                public object Value;
            }

            public IDictionary<string, ITask> GetDependencies(object task) {
                var allDependencies = new Dictionary<string, ITask>();

                IEnumerable<DependencyNameValue> fieldValues = Fields
                    .Select(f => new DependencyNameValue(f.Name, f.GetValue(task)));

                AddDependenciesToSet(allDependencies, fieldValues);
                AddDependencyEnumerationsToSet(allDependencies, fieldValues);

                IEnumerable<DependencyNameValue> propertyValues = Properties
                    .Select(p => new DependencyNameValue(p.Name, p.GetValue(task, new object[0])));

                AddDependenciesToSet(allDependencies, propertyValues);
                AddDependencyEnumerationsToSet(allDependencies, propertyValues);

                return allDependencies;
            }

            private static void AddDependencyEnumerationsToSet(IDictionary<string, ITask> allDependencies, IEnumerable<DependencyNameValue> propertyValues) {
                var edeps = propertyValues
                    .Where(p => p.Value is IEnumerable);

                foreach (DependencyNameValue edep in edeps) {
                    int n = 0;
                    foreach (ITask d in (IEnumerable) edep.Value) {
                        allDependencies.Add(edep.Name + "[" + n + "]", d);
                        n++;
                    }
                }
            }

            private static void AddDependenciesToSet(IDictionary<string, ITask> allDependencies, IEnumerable<DependencyNameValue> depObjects) {
                IEnumerable<DependencyNameValue> deps = depObjects.ToArray()
                    .Where(p => p.Value is ITask);

                foreach (DependencyNameValue dep in deps) {
                    allDependencies.Add(dep.Name, (ITask) dep.Value);
                }
            }
        }

        private Dictionary<Type, TypeDependencyGetter> DependencyGetters;

        public TaskDependencyFinder() {
            DependencyGetters = new Dictionary<Type, TypeDependencyGetter>();
        }

        public IEnumerable<ITask> GetDependenciesFor(object task) {
            return GetDependencyFieldsFor(task).Values;
        }

        public IDictionary<string, ITask> GetDependencyFieldsFor(object task) {
            TypeDependencyGetter getter;

            var type = task.GetType();

            if (!DependencyGetters.TryGetValue(type, out getter)) {
                getter = new TypeDependencyGetter(type);
            }

            return getter.GetDependencies(task);
        }

        private static bool IsDependency(MemberInfo f) {
            return f.GetCustomAttributes(typeof(DependencyAttribute), true).Length > 0;
        }
    }
}