using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace LegacyBounce.Framework.Tests
{
    [TestFixture]
    public class SelectTasksTest
    {
        [Test]
        public void ShouldTaskListOfStringsAndProduceListOfPrinters() {
            var output = new StringWriter();
            Task<IEnumerable<string>> strings = new[] {"one", "two", "three"};
            Task<IEnumerable<FakePrintTask>> printers = strings.SelectTasks(s => new FakePrintTask(output, s));

            printers.TestBuild();

            Assert.That(printers.Value.Count(), Is.EqualTo(3));
            Assert.That(printers.Value, Has.All.AssignableTo(typeof (FakePrintTask)));
            Assert.That(output.ToString(), Is.EqualTo("one;two;three;"));
        }

        [Test]
        public void SelectManyTasksShouldInvokeAllCreatedTasks() {
            var output = new StringWriter();
            Task<IEnumerable<string>> strings = new[] {"one", "two", "three"};
            Task<IEnumerable<FakePrintTask>> printers = strings.SelectManyTasks(s => new [] {new FakePrintTask(output, s), new FakePrintTask(output, "0")});

            printers.TestBuild();

            Assert.That(printers.Value.Count(), Is.EqualTo(6));
            Assert.That(printers.Value, Has.All.AssignableTo(typeof (FakePrintTask)));
            Assert.That(output.ToString(), Is.EqualTo("one;0;two;0;three;0;"));
        }
    }
}