using System.Collections.Generic;
using Xunit;

namespace Bounce.Framework.Tests
{
    public class TaskRunnerTests
    {
        [Fact]
        public void InvokesTaskByWholeName()
        {
            var task = new MockTask {FullName = "asdf"};
            var runner = new TaskRunner();
            var taskParameters = new TaskParameters(new Dictionary<string, string>());

            runner.RunTask("asdf", taskParameters, new[] {task});

            Assert.True(task.WasInvoked);
            Assert.Equal(taskParameters, task.WasInvokedWithTaskParameters);
        }

        [Fact]
        public void CanInvokeTaskByPartialNames()
        {
            var task = new MockTask {FullName = "The.Full.Path.To.Task"};
            var runner = new TaskRunner();

            AssertTaskIsInvokedWithName("The.Full.Path.To.Task", runner, task);
            AssertTaskIsInvokedWithName("Full.Path.To.Task", runner, task);
            AssertTaskIsInvokedWithName("Path.To.Task", runner, task);
            AssertTaskIsInvokedWithName("To.Task", runner, task);
            AssertTaskIsInvokedWithName("Task", runner, task);
        }

        [Fact]
        public void TaskCanBeInvokedWithNameInsensitiveOfCase()
        {
            var task = new MockTask {FullName = "The.Full.Path.To.Task"};
            var runner = new TaskRunner();

            AssertTaskIsInvokedWithName("the.full.path.to.task", runner, task);
            AssertTaskIsInvokedWithName("task", runner, task);
        }

        [Fact]
        public void CannotBeInvokedTaskByPartialNameWithoutSpecificName()
        {
            var task = new MockTask {FullName = "The.Full.Path.To.Task"};
            var runner = new TaskRunner();

            AssertTaskIsNotInvokedWithName("The.Full.Path.To", runner, task);
            AssertTaskIsNotInvokedWithName("Full.Path.To", runner, task);
            AssertTaskIsNotInvokedWithName("Path.To", runner, task);
            AssertTaskIsNotInvokedWithName("To", runner, task);
        }

        private static void AssertTaskIsInvokedWithName(string taskName, ITaskRunner runner, MockTask task)
        {
            runner.RunTask(taskName, new TaskParameters(new Dictionary<string, string>()), new[] {task});

            Assert.True(task.WasInvoked);
        }

        private static void AssertTaskIsNotInvokedWithName(string taskName, ITaskRunner runner, MockTask task)
        {
            var ex = Assert.Throws<NoMatchingTaskException>(() => runner.RunTask(taskName, new TaskParameters(new Dictionary<string, string>()), new[] {task}));
            Assert.Equal("Exception of type 'Bounce.Framework.NoMatchingTaskException' was thrown.", ex.Message);
        }
    }
}