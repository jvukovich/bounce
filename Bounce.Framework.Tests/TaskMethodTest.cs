using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace Bounce.Framework.Tests {
    [TestFixture]
    public class TaskMethodTest {
        public static StringWriter Output;

        [SetUp]
        public void SetUp()
        {
            Output = new StringWriter();
        }

        [Test]
        public void InvokesTaskMethodWithNoParameters()
        {
            var task = new TaskMethod(typeof (FakeTaskClass).GetMethod("Compile"));
            task.Invoke(new Arguments(new Dictionary<string, string>()));

            Assert.That(Output.ToString().Trim(), Is.EqualTo("compiling"));
        }

        [Test]
        public void InvokesTaskMethodWithStringParameter()
        {
            var task = new TaskMethod(typeof(FakeTaskClass).GetMethod("Deploy"));
            task.Invoke(new Arguments(new Dictionary<string, string> { { "dir", @"c:\sites" } }));

            Assert.That(Output.ToString().Trim(), Is.EqualTo(@"deploying c:\sites"));
        }

        [Test]
        public void InvokesTaskMethodWithBooleanParameter()
        {
            var task = new TaskMethod(typeof(FakeTaskClass).GetMethod("Test"));
            task.Invoke(new Arguments(new Dictionary<string, string> { { "fast", @"true" } }));

            Assert.That(Output.ToString().Trim(), Is.EqualTo(@"testing fast"));
        }

        [Test]
        public void InvokesTaskMethodWithOptionalStringParameterNotGiven()
        {
            var task = new TaskMethod(typeof(FakeTaskClass).GetMethod("Optional"));
            task.Invoke(new Arguments(new Dictionary<string, string>()));

            Assert.That(Output.ToString().Trim(), Is.EqualTo(@"optional fileName: stuff.txt"));
        }

        [Test]
        public void InvokesTaskMethodWithOptionalStringParameterGiven()
        {
            var task = new TaskMethod(typeof(FakeTaskClass).GetMethod("Optional"));
            task.Invoke(new Arguments(new Dictionary<string, string>{{"fileName", "thefile.txt"}}));

            Assert.That(Output.ToString().Trim(), Is.EqualTo(@"optional fileName: thefile.txt"));
        }

        [Test]
        public void InvokesTaskMethodWithNullableIntParameterGiven()
        {
            var task = new TaskMethod(typeof(FakeTaskClass).GetMethod("Nullable"));
            task.Invoke(new Arguments(new Dictionary<string, string>{{"port", "80"}}));

            Assert.That(Output.ToString().Trim(), Is.EqualTo(@"port: 80"));
        }

        [Test]
        public void ThrowsExceptionWhenTaskRequiredParameterNotProvided()
        {
            var task = new TaskMethod(typeof(FakeTaskClass).GetMethod("Test"));
            Assert.That(() => task.Invoke(new Arguments(new Dictionary<string, string>())), Throws.InstanceOf<TaskRequiredParameterException>());
        }


        [Test]
        public void ThrowsExceptionWhenCustomTypeCannotBeParsed()
        {
            var task = new TaskMethod(typeof(FakeTaskClass).GetMethod("Bad"));
            Assert.That(() => task.Invoke(new Arguments(new Dictionary<string, string> { { "x", @"something" } })), Throws.InstanceOf<TaskParameterException>());
        }

        [Test]
        public void NullableParameterIsNotRequired() {
            var task = new TaskMethod(typeof(FakeTaskClass).GetMethod("Nullable"));
            Assert.That(task.Parameters.ElementAt(0).IsRequired, Is.False);
        }


        public class FakeTaskClass
        {
            [Task]
            public void Compile()
            {
                Output.WriteLine("compiling");
            }

            [Task]
            public void Deploy(string dir)
            {
                Output.WriteLine("deploying " + dir);
            }

            [Task]
            public void Test(bool fast)
            {
                Output.WriteLine("testing " + (fast ? "fast" : "slow"));
            }

            [Task]
            public void Bad(CustomType x)
            {
            }

            [Task]
            public void Optional(string fileName = "stuff.txt") {
                Output.WriteLine("optional fileName: " + fileName);
            }

            [Task]
            public void Nullable(int? port) {
                Output.WriteLine("port: " + (port.HasValue? port.Value.ToString(): "<nothing>"));
            }
        }

        public class CustomType
        {
        }

        [Test]
        public void EnumeratesTaskNamesForMethodInfo() {
            var taskNames = new TaskMethod(typeof (TasksForTaskNames).GetMethod("DoStuff")).AllNames.ToArray();

            Assert.That(taskNames, Has.Member("DoStuff"));
            Assert.That(taskNames, Has.Member("TasksForTaskNames.DoStuff"));
            Assert.That(taskNames, Has.Member("Tests.TasksForTaskNames.DoStuff"));
            Assert.That(taskNames, Has.Member("Framework.Tests.TasksForTaskNames.DoStuff"));
            Assert.That(taskNames, Has.Member("Bounce.Framework.Tests.TasksForTaskNames.DoStuff"));
        }
    }
}