using Moq;
using NUnit.Framework;

namespace Bounce.Framework.Tests {
    [TestFixture]
    public class CommandLineTasksParametersGeneratorTest {
        [Test]
        public void ShouldGenerateCommandLineParametersFromTasks() {
            var finder = new Mock<IParameterFinder>();
            var task1 = new Mock<ITask>().Object;
            var task2 = new Mock<ITask>().Object;
            var typeParsers = new Mock<ITypeParsers>().Object;

            var param1 = new Mock<IParameter>();
            param1.Setup(p => p.Generate(typeParsers)).Returns("/name1:value1");
            var param2 = new Mock<IParameter>();
            param2.Setup(p => p.Generate(typeParsers)).Returns("/name2:value2");
            var param3 = new Mock<IParameter>();
            param3.Setup(p => p.Generate(typeParsers)).Returns(@"/name3:""value3""");

            finder.Setup(f => f.FindParametersInTask(task1)).Returns(new[] {param1.Object, param2.Object});
            finder.Setup(f => f.FindParametersInTask(task2)).Returns(new[] {param1.Object, param3.Object});

            var generator = new CommandLineTasksParametersGenerator(finder.Object, typeParsers);
            var args = generator.GenerateCommandLineParametersForTasks(new[] {task1, task2});

            Assert.That(args, Is.EqualTo(@"/name1:value1 /name2:value2 /name3:""value3"""));
        }
    }
}