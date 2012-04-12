using System;
using Bounce.Framework.Obsolete;
using NUnit.Framework;

namespace Bounce.Framework.Tests.Obsolete
{
    [TestFixture]
    public class EnvironmentVariablesTest
    {
        [TearDown]
        public void TearDown()
        {
            Environment.SetEnvironmentVariable("stuff", null);
        }

        [Test]
        public void ShouldReturnEnvironmentVariableValue()
        {
            Environment.SetEnvironmentVariable("stuff", "stuff_value");

            var stuff = EnvironmentVariables.Required<string>("stuff");
            Assert.That(stuff.Value, Is.EqualTo("stuff_value"));
        }

        [Test]
        public void DefaultShouldReturnDefaultValueIfVariableNotSet()
        {
            var stuff = EnvironmentVariables.Default("stuff", "default");
            Assert.That(stuff.Value, Is.EqualTo("default"));
        }

        [Test]
        public void RequiredShouldThrowIfVariableNotSet()
        {
            var stuff = EnvironmentVariables.Required<string>("stuff");
            Assert.That(() => stuff.Value, Throws.Exception);
        }
    }
}