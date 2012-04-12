using System;
using System.IO;
using NUnit.Framework;

namespace LegacyBounce.Framework.Tests {
    [TestFixture]
    public class OptionalTaskTest {
        [Test]
        public void IfTrueShouldNotBuildTaskIfConditionFalse() {
            TestOptional(false, false, (task, printTask) => task.IfTrue(printTask));
        }

        [Test]
        public void IfFalseShouldNotBuildTaskIfConditionTrue() {
            TestOptional(true, false, (task, printTask) => task.IfFalse(printTask));
        }

        [Test]
        public void IfTrueShouldBuildTaskIfConditionTrue() {
            TestOptional(true, true, (task, printTask) => task.IfTrue(printTask));
        }

        [Test]
        public void IfFalseShouldBuildTaskIfConditionFalse() {
            TestOptional(false, true, (task, printTask) => task.IfFalse(printTask));
        }

        private void TestOptional(bool conditionValue, bool shouldRun, Func<Task<bool>, IObsoleteTask, IObsoleteTask> getTask) {
            var output = new StringWriter();
            Task<bool> condition = conditionValue;

            var text = "shouldn't see this";
            IObsoleteTask optionalTask = getTask(condition, new FakePrintTask(output, text));

            optionalTask.TestBuild();

            Assert.That(output.ToString(), Is.EqualTo(shouldRun? text + ";": String.Empty));
        }
    }
}