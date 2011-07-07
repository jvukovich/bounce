using System;
using Bounce.Framework.TeamCity;
using NUnit.Framework;

namespace Bounce.Framework.Tests.TeamCity {
    [TestFixture]
    public class TeamCityFactoryTests {
        [Test]
        public void ReturnsVersion5LoggerWhenVersionNotSet()
        {
            SetEnvironmentAndAssert("", typeof (TeamCity5Log));
        }

        [Test]
        public void ReturnsVersion5LoggerWhenVersionIs515(){
            SetEnvironmentAndAssert("5.1.5 (build 15492)", typeof(TeamCity5Log));            
        }

        [Test]
        public void ReturnsStandardLoggerWhenVersionIs65(){
            SetEnvironmentAndAssert("6.5 (build 17534)", typeof(TeamCityLog));
        }

        [Test]
        public void ReturnsStandardLoggerWhenVersionIs651(){
            SetEnvironmentAndAssert("6.5.1 (build 17834)", typeof(TeamCityLog));
        }

        private static void SetEnvironmentAndAssert(string version, Type type)
        {
            var currentEnvironmentVariable = Environment.GetEnvironmentVariable("TEAMCITY_VERSION");
            Environment.SetEnvironmentVariable("TEAMCITY_VERSION", version);
            CreateAndAssertLogType(type);
            Environment.SetEnvironmentVariable("TEAMCITY_VERSION", currentEnvironmentVariable);
        }

        private static void CreateAndAssertLogType(Type type)
        {
            var sut = new TeamCityLogFactory();
            var result = sut.CreateLogForTask(null, null, null, null);
            Assert.That(result, Is.InstanceOf(type));
        }
    }
}