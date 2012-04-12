using System.Collections.Generic;
using System.IO;
using NUnit.Framework;

namespace Bounce.Framework.Tests {
    [TestFixture]
    public class TaskInvokerTest {
        public static StringWriter Output;

        [SetUp]
        public void SetUp() {
            Output = new StringWriter();
        }

        [Test]
        public void InvokesTaskMethodWithNoParameters() {
            var invoker = new TaskInvoker(new Parameters(new Dictionary<string, string>()));
            invoker.InvokeTask(typeof (FakeTaskClass), "Compile");

            Assert.That(Output.ToString().Trim(), Is.EqualTo("compiling"));
        }

        [Test]
        public void InvokesTaskMethodWithStringParameter() {
            var invoker = new TaskInvoker(new Parameters(new Dictionary<string, string>{{"dir", @"c:\sites"}}));
            invoker.InvokeTask(typeof(FakeTaskClass), "Deploy");

            Assert.That(Output.ToString().Trim(), Is.EqualTo(@"deploying c:\sites"));
        }

        [Test]
        public void InvokesTaskMethodWithBooleanParameter() {
            var invoker = new TaskInvoker(new Parameters(new Dictionary<string, string>{{"fast", @"true"}}));
            invoker.InvokeTask(typeof(FakeTaskClass), "Test");

            Assert.That(Output.ToString().Trim(), Is.EqualTo(@"testing fast"));
        }

        [Test]
        public void ThrowsExceptionWhenCustomTypeCannotBeParsed() {
            var invoker = new TaskInvoker(new Parameters(new Dictionary<string, string>{{"x", @"something"}}));
            Assert.That(() => invoker.InvokeTask(typeof(FakeTaskClass), "Bad"), Throws.InstanceOf<TaskParameterException>());
        }

        public class FakeTaskClass {
            [Task]
            public void Compile() {
                Output.WriteLine("compiling");
            }

            [Task]
            public void Deploy(string dir) {
                Output.WriteLine("deploying " + dir);
            }

            [Task]
            public void Test(bool fast) {
                Output.WriteLine("testing " + (fast? "fast": "slow"));
            }

            [Task]
            public void Bad(CustomType x) {
            }
        }

        public class CustomType {
        }
    }
}