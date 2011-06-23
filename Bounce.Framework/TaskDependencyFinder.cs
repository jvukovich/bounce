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
            private IEnumerable<TaskDependencyMember> Fields;
            private IEnumerable<TaskDependencyMember> Properties;

            public TypeDependencyGetter(Type type) {
                var bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

                Fields = type
                    .GetFields(bindingFlags)
                    .Where(MemberHasAttribute<DependencyAttribute>)
                    .Select(prop => new TaskDependencyMember {GetValue = prop.GetValue, Name = prop.Name, CleanAfterBuild = MemberHasAttribute<CleanAfterBuildAttribute>(prop)})
                    .ToArray();

                Properties = type
                    .GetProperties(bindingFlags)
                    .Where(MemberHasAttribute<DependencyAttribute>)
                    .Select(prop => new TaskDependencyMember {GetValue = o => prop.GetValue(o, new object[0]), Name = prop.Name, CleanAfterBuild = MemberHasAttribute<CleanAfterBuildAttribute>(prop)})
                    .ToArray();
            }

            private static bool MemberHasAttribute<T>(MemberInfo memberInfo) {
                return memberInfo.GetCustomAttributes(typeof(T), true).Length > 0;
            }

            class TaskDependencyMember {
                public bool CleanAfterBuild;
                public string Name;
                public Func<object, object> GetValue;
            }

            private class DependencyNameValue {
                private readonly TaskDependencyMember Member;
                private readonly object Obj;
                private bool _hasCachedValue;
                private object _cachedValue;

                public DependencyNameValue(TaskDependencyMember member, object obj) {
                    Member = member;
                    Obj = obj;
                }

                public object Value {
                    get {
                        if (!_hasCachedValue) {
                            _cachedValue = Member.GetValue(Obj);
                            _hasCachedValue = true;
                        }
                        return _cachedValue;
                    }
                }

                public TaskDependency CreateTaskDependency(ITask task, int i) {
                    return CreateTaskDependency(task, Member.Name + "[" + i + "]");
                }

                private TaskDependency CreateTaskDependency(ITask task, string name) {
                    return new TaskDependency(task) {
                                                  Name = name,
                                                  CleanAfterBuild = Member.CleanAfterBuild
                                              };
                }

                public TaskDependency CreateTaskDependency() {
                    return CreateTaskDependency((ITask) Value, Member.Name);
                }
            }

            public IEnumerable<TaskDependency> GetDependencies(object task) {
                var allDependencies = new List<TaskDependency>();

                IEnumerable<DependencyNameValue> fieldValues = Fields
                    .Select(f => new DependencyNameValue(f, task));

                AddDependenciesToSet(allDependencies, fieldValues);
                AddDependencyEnumerationsToSet(allDependencies, fieldValues);

                IEnumerable<DependencyNameValue> propertyValues = Properties
                    .Select(p => new DependencyNameValue(p, task));

                AddDependenciesToSet(allDependencies, propertyValues);
                AddDependencyEnumerationsToSet(allDependencies, propertyValues);

                return allDependencies;
            }

            private static void AddDependencyEnumerationsToSet(ICollection<TaskDependency> allDependencies, IEnumerable<DependencyNameValue> propertyValues) {
                var edeps = propertyValues
                    .Where(p => p.Value is IEnumerable);

                foreach (DependencyNameValue edep in edeps) {
                    int n = 0;
                    foreach (ITask d in (IEnumerable) edep.Value) {
                        allDependencies.Add(edep.CreateTaskDependency(d, n));
                        n++;
                    }
                }
            }

            private static void AddDependenciesToSet(ICollection<TaskDependency> allDependencies, IEnumerable<DependencyNameValue> depObjects) {
                IEnumerable<DependencyNameValue> deps = depObjects.ToArray()
                    .Where(p => p.Value is ITask);

                foreach (DependencyNameValue dep in deps) {
                    allDependencies.Add(dep.CreateTaskDependency());
                }
            }
        }

        private Dictionary<Type, TypeDependencyGetter> DependencyGetters;

        public TaskDependencyFinder() {
            DependencyGetters = new Dictionary<Type, TypeDependencyGetter>();
        }

        public IEnumerable<TaskDependency> GetDependenciesFor(object task) {
            TypeDependencyGetter getter;

            var type = task.GetType();

            if (!DependencyGetters.TryGetValue(type, out getter)) {
                getter = new TypeDependencyGetter(type);
            }

            return getter.GetDependencies(task);
        }
    }
}