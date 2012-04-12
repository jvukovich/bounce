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
            task.Invoke(new Parameters(new Dictionary<string, string>()));

            Assert.That(Output.ToString().Trim(), Is.EqualTo("compiling"));
        }

        [Test]
        public void InvokesTaskMethodWithStringParameter()
        {
            var task = new TaskMethod(typeof(FakeTaskClass).GetMethod("Deploy"));
            task.Invoke(new Parameters(new Dictionary<string, string> { { "dir", @"c:\sites" } }));

            Assert.That(Output.ToString().Trim(), Is.EqualTo(@"deploying c:\sites"));
        }

        [Test]
        public void InvokesTaskMethodWithBooleanParameter()
        {
            var task = new TaskMethod(typeof(FakeTaskClass).GetMethod("Test"));
            task.Invoke(new Parameters(new Dictionary<string, string> { { "fast", @"true" } }));

            Assert.That(Output.ToString().Trim(), Is.EqualTo(@"testing fast"));
        }

        [Test]
        public void ThrowsExceptionWhenTaskRequiredParameterNotProvided()
        {
            var task = new TaskMethod(typeof(FakeTaskClass).GetMethod("Test"));
            Assert.That(() => task.Invoke(new Parameters(new Dictionary<string, string>())), Throws.InstanceOf<TaskRequiredParameterException>());
        }


        [Test]
        public void ThrowsExceptionWhenCustomTypeCannotBeParsed()
        {
            var task = new TaskMethod(typeof(FakeTaskClass).GetMethod("Bad"));
            Assert.That(() => task.Invoke(new Parameters(new Dictionary<string, string> { { "x", @"something" } })), Throws.InstanceOf<TaskParameterException>());
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