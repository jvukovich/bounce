using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace Bounce.Framework.Tests {
    [TestFixture]
    public class TaskMethodTest {
        public static StringWriter Output;
        public IDependencyResolver Resolver;

        [SetUp]
        public void SetUp()
        {
            Output = new StringWriter();
            Resolver = new SimpleDependencyResolver();
        }

        [Test]
        public void InvokesTaskMethodWithNoParameters()
        {
            var task = new TaskMethod(typeof (FakeTaskClass).GetMethod("Compile"), Resolver);
            task.Invoke(new Arguments(new Dictionary<string, string>()));

            Assert.That(Output.ToString().Trim(), Is.EqualTo("compiling"));
        }

        [Test]
        public void InvokesTaskMethodWithStringParameter()
        {
            var task = new TaskMethod(typeof(FakeTaskClass).GetMethod("Deploy"), Resolver);
            task.Invoke(new Arguments(new Dictionary<string, string> { { "dir", @"c:\sites" } }));

            Assert.That(Output.ToString().Trim(), Is.EqualTo(@"deploying c:\sites"));
        }

        [Test]
        public void InvokesTaskMethodWithBooleanParameter()
        {
            var task = new TaskMethod(typeof(FakeTaskClass).GetMethod("Test"), Resolver);
            task.Invoke(new Arguments(new Dictionary<string, string> { { "fast", @"true" } }));

            Assert.That(Output.ToString().Trim(), Is.EqualTo(@"testing fast"));
        }

        [Test]
        public void InvokesTaskMethodWithOptionalStringParameterNotGiven()
        {
            var task = new TaskMethod(typeof(FakeTaskClass).GetMethod("Optional"), Resolver);
            task.Invoke(new Arguments(new Dictionary<string, string>()));

            Assert.That(Output.ToString().Trim(), Is.EqualTo(@"optional fileName: stuff.txt"));
        }

        [Test]
        public void InvokesTaskMethodWithOptionalStringParameterGiven()
        {
            var task = new TaskMethod(typeof(FakeTaskClass).GetMethod("Optional"), Resolver);
            task.Invoke(new Arguments(new Dictionary<string, string>{{"fileName", "thefile.txt"}}));

            Assert.That(Output.ToString().Trim(), Is.EqualTo(@"optional fileName: thefile.txt"));
        }

        [Test]
        public void InvokesTaskMethodWithNullableIntParameterGiven()
        {
            var task = new TaskMethod(typeof(FakeTaskClass).GetMethod("Nullable"), Resolver);
            task.Invoke(new Arguments(new Dictionary<string, string>{{"port", "80"}}));

            Assert.That(Output.ToString().Trim(), Is.EqualTo(@"port: 80"));
        }

        [Test]
        public void ThrowsExceptionWhenTaskRequiredParameterNotProvided()
        {
            var task = new TaskMethod(typeof(FakeTaskClass).GetMethod("Test"), Resolver);
            Assert.That(() => task.Invoke(new Arguments(new Dictionary<string, string>())), Throws.InstanceOf<TaskRequiredParameterException>());
        }


        [Test]
        public void ThrowsExceptionWhenCustomTypeCannotBeParsed()
        {
            var task = new TaskMethod(typeof(FakeTaskClass).GetMethod("Bad"), Resolver);
            Assert.That(() => task.Invoke(new Arguments(new Dictionary<string, string> { { "x", @"something" } })), Throws.InstanceOf<TaskParameterException>());
        }

        [Test]
        public void NullableParameterIsNotRequired() {
            var task = new TaskMethod(typeof(FakeTaskClass).GetMethod("Nullable"), Resolver);
            Assert.That(task.Parameters.ElementAt(0).IsRequired, Is.False);
        }

        [Test]
        public void SameExceptionIsThrownWhenTaskThrows() {
            var task = new TaskMethod(typeof(FakeTaskClass).GetMethod("Throws"), Resolver);
            Assert.That(() => task.Invoke(new Arguments(new Dictionary<string, string>())), Throws.InstanceOf<BadException>());
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

            [Task]
            public void Throws() {
                throw new BadException();
            }
        }

        public class BadException : Exception {}

        public class CustomType
        {
        }

        [Test]
        public void EnumeratesTaskNamesForMethodInfo() {
            var taskNames = new TaskMethod(typeof(TasksForTaskNames).GetMethod("DoStuff"), Resolver).AllNames.ToArray();

            Assert.That(taskNames, Has.Member("DoStuff"));
            Assert.That(taskNames, Has.Member("TasksForTaskNames.DoStuff"));
            Assert.That(taskNames, Has.Member("Tests.TasksForTaskNames.DoStuff"));
            Assert.That(taskNames, Has.Member("Framework.Tests.TasksForTaskNames.DoStuff"));
            Assert.That(taskNames, Has.Member("Bounce.Framework.Tests.TasksForTaskNames.DoStuff"));
        }
    }
}