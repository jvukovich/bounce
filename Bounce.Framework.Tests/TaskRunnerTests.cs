using System.Collections.Generic;
using NUnit.Framework;

namespace Bounce.Framework.Tests {
    [TestFixture]
    public class TaskRunnerTests {
        [Test]
        public void InvokesTaskByWholeName() {
            var task = new MockTask {FullName = "asdf"};
            var runner = new TaskRunner();
            var taskParameters = new TaskParameters(new Dictionary<string, string>());
            runner.RunTask("asdf", taskParameters, new [] {task});

            Assert.That(task.WasInvoked, Is.True);
            Assert.That(task.WasInvokedWithTaskParameters, Is.SameAs(taskParameters));
        }

        [Test]
        public void CanInvokeTaskByPartialNames() {
            var task = new MockTask {FullName = "The.Full.Path.To.Task"};
            var runner = new TaskRunner();

            AssertTaskIsInvokedWithName("The.Full.Path.To.Task", runner, task);
            AssertTaskIsInvokedWithName("Full.Path.To.Task", runner, task);
            AssertTaskIsInvokedWithName("Path.To.Task", runner, task);
            AssertTaskIsInvokedWithName("To.Task", runner, task);
            AssertTaskIsInvokedWithName("Task", runner, task);
        }

        [Test]
        public void TaskCanBeInvokedWithNameInsensitiveOfCase() {
            var task = new MockTask {FullName = "The.Full.Path.To.Task"};
            var runner = new TaskRunner();

            AssertTaskIsInvokedWithName("the.full.path.to.task", runner, task);
            AssertTaskIsInvokedWithName("task", runner, task);
        }

        [Test]
        public void CannotBeInvokedTaskByPartialNameWithoutSpecificName() {
            var task = new MockTask {FullName = "The.Full.Path.To.Task"};
            var runner = new TaskRunner();

            AssertTaskIsNotInvokedWithName("The.Full.Path.To", runner, task);
            AssertTaskIsNotInvokedWithName("Full.Path.To", runner, task);
            AssertTaskIsNotInvokedWithName("Path.To", runner, task);
            AssertTaskIsNotInvokedWithName("To", runner, task);
        }

        private static void AssertTaskIsInvokedWithName(string taskName, TaskRunner runner, MockTask task) {
            runner.RunTask(taskName, new TaskParameters(new Dictionary<string, string>()), new[] {task});
            Assert.That(task.WasInvoked, Is.True);
        }

        private static void AssertTaskIsNotInvokedWithName(string taskName, TaskRunner runner, MockTask task) {
            Assert.That(
                () => runner.RunTask(taskName, new TaskParameters(new Dictionary<string, string>()), new[] {task}),
                Throws.InstanceOf<NoMatchingTaskException>());
        }
    }
}