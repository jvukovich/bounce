using System.Collections.Generic;
using Moq;
using NUnit.Framework;

namespace LegacyBounce.Framework.Tests {
    [TestFixture]
    public class CommandLineTasksParametersGeneratorTest {
        [Test]
        public void ShouldGenerateCommandLineParametersFromTasks() {
            var finder = new Mock<IParameterFinder>();
            var task1 = new Mock<IObsoleteTask>().Object;
            var task2 = new Mock<IObsoleteTask>().Object;
            var typeParsers = new Mock<ITypeParsers>().Object;

            var param1 = new FakeParameter {Name = "name1", GeneratedValue = "/name1:value1"};
            var param2 = new FakeParameter {Name = "name2", GeneratedValue = "/name2:value2"};
            var param3 = new FakeParameter {Name = "name3", GeneratedValue = @"""/name3:value 3"""};

            finder.Setup(f => f.FindParametersInTask(task1)).Returns(new[] {param1, param2});
            finder.Setup(f => f.FindParametersInTask(task2)).Returns(new[] {param1, param3});

            var generator = new CommandLineTasksParametersGenerator(finder.Object, typeParsers);

            FakeParameter param2Override = new FakeParameter {Name = "name2", GeneratedValue = "/name2:anothervalue2"};
            FakeParameter notUsedParameterOverride = new FakeParameter {Name = "notused", GeneratedValue = "/notused:notusedvalue"};

            var args = generator.GenerateCommandLineParametersForTasks(new[] {param1, param2, param3}, new [] {param2Override, notUsedParameterOverride});

            Assert.That(args, Is.EqualTo(@"/name1:value1 /name2:anothervalue2 ""/name3:value 3"" /notused:notusedvalue"));
        }

        class FakeParameter : IParameter {
            public virtual void Parse(string parameterValue, ITypeParsers typeParsers) {
            }

            public virtual string Generate(ITypeParsers typeParsers) {
                return GeneratedValue;
            }

            public string GeneratedValue { get; set; }

            public string Name { get; set; }
            public bool Required { get; set; }
            public bool HasValue { get; set; }
            public IEnumerable<object> AvailableValues { get; set; }
            public object DefaultValue { get; set; }
        }
    }
}