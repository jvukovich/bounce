using NUnit.Framework;

namespace Bounce.Framework.Tests {
    [TestFixture]
    public class ParameterFinderTest {
        [Test]
        public void ShouldReturnAllParametersForTask() {
            var param1 = new Parameter<string>();
            var param2 = new Parameter<string>();
            var task = new TaskWithParameters {Param = param1, Task = new TaskWithParameters {Param = param2}};

            var finder = new ParameterFinder();
            var parametersFound = finder.FindParametersInTask(task);

            Assert.That(parametersFound, Is.EquivalentTo(new[] {param1, param2}));
        }

        class TaskWithParameters : Task {
            [Dependency] public Future<string> Param;
            [Dependency] public ITask Task;
        }
    }
}