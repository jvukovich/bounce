using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Xunit;

namespace Bounce.Framework.Tests
{
    public class TaskMethodTest
    {
        private static StringWriter output;
        private readonly IDependencyResolver resolver;

        public TaskMethodTest()
        {
            output = new StringWriter();
            resolver = new SimpleDependencyResolver();
        }

        [Fact]
        public void InvokesTaskMethodWithNoParameters()
        {
            var task = new TaskMethod(typeof(FakeTaskClass).GetMethod("Compile"), resolver);

            task.Invoke(new TaskParameters(new Dictionary<string, string>()));

            Assert.Equal("compiling", output.ToString().Trim());
        }

        [Fact]
        public void InvokesTaskMethodWithStringParameter()
        {
            var task = new TaskMethod(typeof(FakeTaskClass).GetMethod("Deploy"), resolver);

            task.Invoke(new TaskParameters(new Dictionary<string, string> {{"dir", @"c:\sites"}}));

            Assert.Equal(@"deploying c:\sites", output.ToString().Trim());
        }

        [Fact]
        public void InvokesTaskMethodWithBooleanParameter()
        {
            var task = new TaskMethod(typeof(FakeTaskClass).GetMethod("Test"), resolver);

            task.Invoke(new TaskParameters(new Dictionary<string, string> {{"fast", "true"}}));

            Assert.Equal("testing fast", output.ToString().Trim());
        }

        [Fact]
        public void InvokesTaskMethodWithOptionalStringParameterNotGiven()
        {
            var task = new TaskMethod(typeof(FakeTaskClass).GetMethod("Optional"), resolver);

            task.Invoke(new TaskParameters(new Dictionary<string, string>()));

            Assert.Equal("optional fileName: stuff.txt", output.ToString().Trim());
        }

        [Fact]
        public void InvokesTaskMethodWithOptionalStringParameterGiven()
        {
            var task = new TaskMethod(typeof(FakeTaskClass).GetMethod("Optional"), resolver);

            task.Invoke(new TaskParameters(new Dictionary<string, string> {{"fileName", "thefile.txt"}}));

            Assert.Equal("optional fileName: thefile.txt", output.ToString().Trim());
        }

        [Fact]
        public void InvokesTaskMethodWithNullableIntParameterGiven()
        {
            var task = new TaskMethod(typeof(FakeTaskClass).GetMethod("Nullable"), resolver);

            task.Invoke(new TaskParameters(new Dictionary<string, string> {{"port", "80"}}));

            Assert.Equal("port: 80", output.ToString().Trim());
        }

        [Fact]
        public void ThrowsExceptionWhenTaskRequiredParameterNotProvided()
        {
            var task = new TaskMethod(typeof(FakeTaskClass).GetMethod("Test"), resolver);
            var ex = Assert.Throws<Exception>(() => task.Invoke(new TaskParameters(new Dictionary<string, string>())));

            Assert.Contains("required parameter 'fast' not given", ex.Message);
        }

        [Fact]
        public void ThrowsExceptionWhenCustomTypeCannotBeParsed()
        {
            var task = new TaskMethod(typeof(FakeTaskClass).GetMethod("Bad"), resolver);
            var ex = Assert.Throws<Exception>(() => task.Invoke(new TaskParameters(new Dictionary<string, string> {{"x", "something"}})));

            Assert.Contains("could not parse 'something' for type", ex.Message);
        }

        [Fact]
        public void NullableParameterIsNotRequired()
        {
            var task = new TaskMethod(typeof(FakeTaskClass).GetMethod("Nullable"), resolver);

            Assert.False(task.Parameters.ElementAt(0).IsRequired);
        }

        [Fact]
        public void TaskExceptionIsThrownWhenTaskThrows()
        {
            var task = new TaskMethod(typeof(FakeTaskClass).GetMethod("Throws"), resolver);
            var ex = Assert.Throws<TargetInvocationException>(() => task.Invoke(new TaskParameters(new Dictionary<string, string>())));

            Assert.Contains("Exception has been thrown by the target", ex.Message);
        }

        public class FakeTaskClass
        {
            [Task]
            public void Compile()
            {
                output.WriteLine("compiling");
            }

            [Task]
            public void Deploy(string dir)
            {
                output.WriteLine("deploying " + dir);
            }

            [Task]
            public void Test(bool fast)
            {
                output.WriteLine("testing " + (fast ? "fast" : "slow"));
            }

            [Task]
            public void Bad(CustomType x)
            {
            }

            [Task]
            public void Optional(string fileName = "stuff.txt")
            {
                output.WriteLine("optional fileName: " + fileName);
            }

            [Task]
            public void Nullable(int? port)
            {
                output.WriteLine("port: " + (port.HasValue ? port.Value.ToString() : "<nothing>"));
            }

            [Task]
            public void Throws()
            {
                throw new BadException();
            }
        }

        private class BadException : Exception
        {
        }

        public abstract class CustomType
        {
        }
    }
}