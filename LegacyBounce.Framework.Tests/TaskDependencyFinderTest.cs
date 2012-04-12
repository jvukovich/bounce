using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;

namespace LegacyBounce.Framework.Tests {
    [TestFixture]
    public class TaskDependencyFinderTest {
        [Test]
        public void ShouldReturnFieldDependenciesForTaskSubClass() {
            AssertThatCreatedObjectReturnsDependencies((a, b, c) => new TaskWithFields(a, b, c), false);
        }

        [Test]
        public void ShouldReturnPropertyDependenciesForTaskSubClass() {
            AssertThatCreatedObjectReturnsDependencies((a, b, c) => new TaskWithProperties(() => a, b, c), false);
        }

        [Test]
        public void ShouldReturnEnumerablePropertyDependenciesForTaskSubClass() {
            AssertThatCreatedObjectReturnsDependencies((a, b, c) => new TaskWithEnumerationsInProperties(() => a, b, c), true);
        }

        [Test]
        public void ShouldReturnEnumerableFieldDependenciesForTaskSubClass() {
            AssertThatCreatedObjectReturnsDependencies((a, b, c) => new TaskWithEnumerationsInFields(a, b, c), true);
        }

        [Test]
        public void Iis6WebSiteBindingShouldReturnPortAsDependency()
        {
            var port = new Mock<Task<int>>().Object;
            var deps = new TaskDependencyFinder().GetDependenciesFor(new Iis6WebSiteBinding {Port = port});
            Assert.That(deps.Select(d => d.Task).ToArray(), Has.Member(port));
        }

        [Test]
        public void DependencyShouldIndicateCleanAfterBuild() {
            var task = new TaskWithCleanAfterBuildDependencies();
            var deps = new TaskDependencyFinder().GetDependenciesFor(task);

            Assert.That(deps.First(d => d.Task == task.A).CleanAfterBuild, Is.True);
            Assert.That(deps.First(d => d.Task == task.B).CleanAfterBuild, Is.False);
            Assert.That(deps.Where(d => d.Task == task.C), Is.Empty);
        }

        class TaskWithCleanAfterBuildDependencies : Task {
            [Dependency, CleanAfterBuild] public IObsoleteTask A = new Mock<IObsoleteTask>().Object;
            [Dependency] public IObsoleteTask B = new Mock<IObsoleteTask>().Object;
            [CleanAfterBuild] public IObsoleteTask C = new Mock<IObsoleteTask>().Object;
        }

        private void AssertThatCreatedObjectReturnsDependencies(Func<IObsoleteTask,IObsoleteTask,SomeTask, IObsoleteTask> createObject, bool areEnumerations) {
            var finder = new TaskDependencyFinder();

            var a = new Mock<IObsoleteTask>().Object;
            var b = new Mock<IObsoleteTask>().Object;
            var c = new SomeTask();

            var task = createObject(a, b, c);

            IEnumerable<TaskDependency> deps = finder.GetDependenciesFor(task);

            Assert.That(deps.Single(d => d.Name == MakeEnumerationProperty("A", areEnumerations)).Task, Is.SameAs(a));
            Assert.That(deps.Single(d => d.Name == MakeEnumerationProperty("B", areEnumerations)).Task, Is.SameAs(b));
            Assert.That(deps.Single(d => d.Name == MakeEnumerationProperty("C", areEnumerations)).Task, Is.SameAs(c));
            Assert.That(deps.Select(d => d.Task), Is.EquivalentTo(new [] {a, b, c}));
        }

        private string MakeEnumerationProperty(string s, bool enumerations)
        {
            return enumerations ? s + "[0]" : s;
        }

        class SomeTask : Task {
        }

        class TaskWithFields : SomeTask {
            [Dependency]
            public IObsoleteTask A;
            [Dependency]
            private IObsoleteTask B;
            [Dependency]
            protected SomeTask C;

            public TaskWithFields(IObsoleteTask a, IObsoleteTask b, SomeTask c) {
                A = a;
                B = b;
                C = c;
            }
        }

        class TaskWithProperties : SomeTask {
            private Func<IObsoleteTask> getA;

            [Dependency]
            public IObsoleteTask A {
                get {
                    return getA();
                }
            }

            [Dependency]
            private IObsoleteTask B { get; set; }
            [Dependency]
            protected SomeTask C { get; set; }

            public TaskWithProperties(Func<IObsoleteTask> a, IObsoleteTask b, SomeTask c) {
                getA = a;
                B = b;
                C = c;
            }
        }

        class TaskWithEnumerationsInProperties : SomeTask {
            private Func<IObsoleteTask> getA;

            [Dependency]
            public IEnumerable<IObsoleteTask> A {
                get {
                    return new[] {getA()};
                }
            }

            [Dependency]
            private IObsoleteTask[] B { get; set; }
            [Dependency]
            protected List<SomeTask> C { get; set; }

            public TaskWithEnumerationsInProperties(Func<IObsoleteTask> a, IObsoleteTask b, SomeTask c) {
                getA = a;
                B = new[] {b};
                C = new List<SomeTask> {c};
            }
        }

        class TaskWithEnumerationsInFields : SomeTask {
            [Dependency] public IEnumerable<IObsoleteTask> A;
            [Dependency] private IObsoleteTask[] B;
            [Dependency] protected List<SomeTask> C;

            public TaskWithEnumerationsInFields(IObsoleteTask a, IObsoleteTask b, SomeTask c) {
                A = new [] {a};
                B = new[] {b};
                C = new List<SomeTask> {c};
            }
        }
    }
}