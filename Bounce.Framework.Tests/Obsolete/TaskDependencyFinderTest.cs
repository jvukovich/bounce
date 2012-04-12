using System;
using System.Collections.Generic;
using System.Linq;
using Bounce.Framework.Obsolete;
using Moq;
using NUnit.Framework;

namespace Bounce.Framework.Tests.Obsolete {
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
            [Dependency, CleanAfterBuild] public ITask A = new Mock<ITask>().Object;
            [Dependency] public ITask B = new Mock<ITask>().Object;
            [CleanAfterBuild] public ITask C = new Mock<ITask>().Object;
        }

        private void AssertThatCreatedObjectReturnsDependencies(Func<ITask,ITask,SomeTask, ITask> createObject, bool areEnumerations) {
            var finder = new TaskDependencyFinder();

            var a = new Mock<ITask>().Object;
            var b = new Mock<ITask>().Object;
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
            public ITask A;
            [Dependency]
            private ITask B;
            [Dependency]
            protected SomeTask C;

            public TaskWithFields(ITask a, ITask b, SomeTask c) {
                A = a;
                B = b;
                C = c;
            }
        }

        class TaskWithProperties : SomeTask {
            private Func<ITask> getA;

            [Dependency]
            public ITask A {
                get {
                    return getA();
                }
            }

            [Dependency]
            private ITask B { get; set; }
            [Dependency]
            protected SomeTask C { get; set; }

            public TaskWithProperties(Func<ITask> a, ITask b, SomeTask c) {
                getA = a;
                B = b;
                C = c;
            }
        }

        class TaskWithEnumerationsInProperties : SomeTask {
            private Func<ITask> getA;

            [Dependency]
            public IEnumerable<ITask> A {
                get {
                    return new[] {getA()};
                }
            }

            [Dependency]
            private ITask[] B { get; set; }
            [Dependency]
            protected List<SomeTask> C { get; set; }

            public TaskWithEnumerationsInProperties(Func<ITask> a, ITask b, SomeTask c) {
                getA = a;
                B = new[] {b};
                C = new List<SomeTask> {c};
            }
        }

        class TaskWithEnumerationsInFields : SomeTask {
            [Dependency] public IEnumerable<ITask> A;
            [Dependency] private ITask[] B;
            [Dependency] protected List<SomeTask> C;

            public TaskWithEnumerationsInFields(ITask a, ITask b, SomeTask c) {
                A = new [] {a};
                B = new[] {b};
                C = new List<SomeTask> {c};
            }
        }
    }
}