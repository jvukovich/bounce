using Bounce.Framework;
using Bounce.TestHelpers;
using NUnit.Framework;

namespace Bounce.Console.Tests {
    [TestFixture]
    public class RunningTasksTests {
        [Test]
        public void CanRunATask() {
            var shell = new Shell(new FakeLog());

            ProcessOutput output = null;

            FileSystemUtils.Pushd(@"..\..\..\TestBounceAssembly", () => output = shell.Exec(@"..\Bounce.Console.Tests\bin\Debug\bounce.exe", "Task1"));

            Assert.That(output, Is.Not.Null);
            Assert.That(output.ExitCode, Is.EqualTo(0));
            Assert.That(output.Error.Trim(), Is.EqualTo(""));
            Assert.That(output.Output, Is.StringContaining("task1"));
        }

        [Test]
        public void CanRunATaskWithArguments() {
            var shell = new Shell(new FakeLog());

            ProcessOutput output = null;

            FileSystemUtils.Pushd(@"..\..\..\TestBounceAssembly", () => output = shell.Exec(@"..\Bounce.Console.Tests\bin\Debug\bounce.exe", "Task2 /a b"));

            Assert.That(output, Is.Not.Null);
            Assert.That(output.ExitCode, Is.EqualTo(0));
            Assert.That(output.Error.Trim(), Is.EqualTo(""));
            Assert.That(output.Output, Is.StringContaining("task2 a=b"));
        }
    }
}