using System;
using System.IO;

namespace Bounce.Framework.TeamCity
{
    /// <summary>
    /// Publishes artifacts to teamcity, as per http://confluence.jetbrains.net/display/TCD65/Build+Script+Interaction+with+TeamCity#BuildScriptInteractionwithTeamCity-PublishingArtifactswhiletheBuildisStillinProgress
    /// </summary>
    public class TeamCityArtifactPublisher
    {
        public void Publish(string artifactPath)
        {
            Console.WriteLine(@"##teamcity[publishArtifacts '{0}']", artifactPath);
        }
    }
}