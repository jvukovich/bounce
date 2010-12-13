using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Moq;
using NUnit.Framework;

namespace Bounce.Framework.Tests
{
    [TestFixture]
    public class MultiTaskTest
    {
        [Test]
        public void ShouldInvokeEachTask()
        {
            StringWriter output = new StringWriter();
            var multiTask = new FakeMultiTask(output);

            var bounce = new FakeBounce();

            multiTask.Invoke(BounceCommand.Build, bounce);

            Assert.That(output.ToString(), Is.EqualTo("one;two;three;"));
        }

        class FakeMultiTask : MultiTask
        {
            private StringWriter Output;

            public FakeMultiTask(StringWriter output)
            {
                Output = output;
            }

            public override IEnumerable<ITask> GetTasks(IBounce bounce)
            {
                return new[] {"one", "two", "three"}.Select(name => (ITask) new FakePrintTask(Output, name));
            }
        }

        class FakePrintTask : Task
        {
            private StringWriter Output;
            private readonly string Name;

            public FakePrintTask(StringWriter output, string name)
            {
                Output = output;
                Name = name;
            }

            public override void Build(IBounce bounce) {
                Output.Write(Name + ";");
            }
        }
    }
}