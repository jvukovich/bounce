using Bounce.Framework;
using Xunit;

namespace Bounce.Console.Tests
{
    public class RunningTasksTests
    {
        [Fact]
        public void CanRunATask()
        {
            var shell = new Shell(new FakeLog());

            ProcessOutput output = null;

            FileSystemUtils.Pushd(@"..\..\..\TestBounceAssembly", () => output = shell.Exec(@"..\Bounce.Console\bin\Debug\bounce.exe", "Task1"));

            Assert.NotNull(output);
            Assert.Equal(0, output.ExitCode);
            Assert.Equal(string.Empty, output.Error.Trim());
            Assert.Contains("task1", output.Output);
        }

        [Fact]
        public void CanRunATaskWithArguments()
        {
            var shell = new Shell(new FakeLog());

            ProcessOutput output = null;

            FileSystemUtils.Pushd(@"..\..\..\TestBounceAssembly", () => output = shell.Exec(@"..\Bounce.Console\bin\Debug\bounce.exe", "Task2 /a b"));

            Assert.NotNull(output);
            Assert.Equal(0, output.ExitCode);
            Assert.Equal(string.Empty, output.Error.Trim());
            Assert.Contains("task2 a=b", output.Output);
        }

        [Fact]
        public void CanRunMultipleTasksWithMultipleArguments()
        {
            var shell = new Shell(new FakeLog());

            ProcessOutput output = null;

            FileSystemUtils.Pushd(@"..\..\..\TestBounceAssembly", () => output = shell.Exec(@"..\Bounce.Console\bin\Debug\bounce.exe", "Task3 Task4 Task5 /a a /b:b /c c"));

            Assert.NotNull(output);
            Assert.Equal(0, output.ExitCode);
            Assert.Equal(string.Empty, output.Error.Trim());
            Assert.Contains("task3 a=a", output.Output);
            Assert.Contains("task4 b=b", output.Output);
            Assert.Contains("task5 a=a, b=b, c=c", output.Output);
        }
    }
}