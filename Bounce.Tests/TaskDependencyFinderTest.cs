using System;
using System.Collections.Generic;
using Bounce.Framework;
using Moq;
using NUnit.Framework;

namespace Bounce.Tests {
    [TestFixture]
    public class TaskDependencyFinderTest {
        [Test]
        public void ShouldReturnFieldDependenciesForTaskSubClass() {
            AssertThatCreatedObjectReturnsDependencies((a, b, c) => new TaskWithFields(a, b, c));
        }

        [Test]
        public void ShouldReturnPropertyDependenciesForTaskSubClass() {
            AssertThatCreatedObjectReturnsDependencies((a, b, c) => new TaskWithProperties(() => a, b, c));
        }

        [Test]
        public void ShouldReturnEnumerablePropertyDependenciesForTaskSubClass() {
            AssertThatCreatedObjectReturnsDependencies((a, b, c) => new TaskWithEnumerationsInProperties(() => a, b, c));
        }

        [Test]
        public void ShouldReturnEnumerableFieldDependenciesForTaskSubClass() {
            AssertThatCreatedObjectReturnsDependencies((a, b, c) => new TaskWithEnumerationsInFields(a, b, c));
        }

        private void AssertThatCreatedObjectReturnsDependencies(Func<ITask,ITask,SomeTask, ITask> createObject) {
            var finder = new TaskDependencyFinder();

            var a = new Mock<ITask>().Object;
            var b = new Mock<ITask>().Object;
            var c = new SomeTask();

            var task = createObject(a, b, c);

            IEnumerable<ITask> deps = finder.GetDependenciesFor(task);

            Assert.That(deps, Is.EquivalentTo(new [] {a, b, c}));
        }

        class SomeTask : ITask {
            public IEnumerable<ITask> Dependencies {
                get { throw new System.NotImplementedException(); }
            }

            public void BeforeBuild(IBounce bounce) {
                throw new System.NotImplementedException();
            }

            public void Build(IBounce bounce) {
                throw new System.NotImplementedException();
            }

            public void Clean(IBounce bounce) {
                throw new System.NotImplementedException();
            }
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