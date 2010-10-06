using System.IO;
using NUnit.Framework;

namespace Bounce.Framework.Tests {
    [TestFixture]
    public class TeamCityMSBuildLoggerTest {
        [Test]
        public void ShouldBeWarning() {
            TextWriter output = new StringWriter();

            var logger = new TeamCityMsBuildLogger("SomeSolution.sln", output);

            logger.CommandOutput("BounceRunner.cs(103,32): warning CS0168: The variable 'e' is declared but never used");

            Assert.That(output.ToString(), Is.EqualTo("yep, it's a warning!\r\n"));
        }
    }
}