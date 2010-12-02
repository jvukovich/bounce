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

            public IEnumerable<ITask> GetDependencies(object task) {
                var allDependencies = new HashSet<ITask>();

                IEnumerable<object> fieldValues = Fields
                    .Select(f => f.GetValue(task));

                AddDependenciesToSet(allDependencies, fieldValues);
                AddDependencyEnumerationsToSet(allDependencies, fieldValues);

                IEnumerable<object> propertyValues = Properties
                    .Select(p => p.GetValue(task, new object[0]));

                AddDependenciesToSet(allDependencies, propertyValues);
                AddDependencyEnumerationsToSet(allDependencies, propertyValues);

                return allDependencies;
            }

            private static void AddDependencyEnumerationsToSet(HashSet<ITask> allDependencies, IEnumerable<object> propertyValues) {
                var edeps = propertyValues
                    .Where(p => p is IEnumerable)
                    .Cast<IEnumerable>();

                foreach (IEnumerable edep in edeps) {
                    foreach (ITask d in edep) {
                        allDependencies.Add(d);
                    }
                }
            }

            private static void AddDependenciesToSet(HashSet<ITask> allDependencies, IEnumerable<object> depObjects) {
                IEnumerable<ITask> deps = depObjects.ToArray()
                    .Where(p => p is ITask)
                    .Cast<ITask>();

                foreach (ITask dep in deps) {
                    allDependencies.Add(dep);
                }
            }
        }

        private Dictionary<Type, TypeDependencyGetter> DependencyGetters;

        public TaskDependencyFinder() {
            DependencyGetters = new Dictionary<Type, TypeDependencyGetter>();
        }

        public IEnumerable<ITask> GetDependenciesFor(object task) {
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