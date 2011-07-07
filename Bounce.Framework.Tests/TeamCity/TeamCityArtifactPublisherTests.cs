using System;
using System.IO;
using Bounce.Framework.TeamCity;
using Moq;
using NUnit.Framework;

namespace Bounce.Framework.Tests.TeamCity
{
    [TestFixture]
    public class TeamCityArtifactPublisherTests
    {
        [Test]
        public void ShouldOutputTeamCityPublishCommandToConsole()
        {
            var bounce = new Mock<IBounce>();
            var fakeConsole = new StringWriter();
            var sut = new TeamCityArtifactPublisher {ArtifactPath = @"c:\temp\foobar.txt", Output = fakeConsole};
            sut.Build(bounce.Object);
            Assert.That(fakeConsole.ToString(), Is.EqualTo(@"##teamcity[publishArtifacts 'c:\temp\foobar.txt']" + Environment.NewLine));
        }
    }
}