using System.IO;
using NUnit.Framework;

namespace Bounce.Framework.Tests {
    [TestFixture]
    public class OptionalTaskTest {
        [Test]
        public void ShouldNotBuildTaskIfConditionFalse() {
            var output = new StringWriter();
            Future<bool> condition = false;

            ITask optionalTask = condition.OptionalTask(() => new FakePrintTask(output, "shouldn't see this"));

            optionalTask.Invoke(BounceCommand.Build, new FakeBounce());

            Assert.That(output.ToString(), Is.Empty);
        }

        [Test]
        public void ShouldBuildTaskIfConditionTrue() {
            var output = new StringWriter();
            Future<bool> condition = true;

            ITask optionalTask = condition.OptionalTask(() => new FakePrintTask(output, "task_built"));

            optionalTask.Invoke(BounceCommand.Build, new FakeBounce());

            Assert.That(output.ToString(), Is.EqualTo("task_built;"));
        }
    }
}