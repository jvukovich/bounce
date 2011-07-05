using System;
using System.IO;

namespace Bounce.Framework.TeamCity
{
    /// <summary>
    /// Publishes artifacts to teamcity, as per http://confluence.jetbrains.net/display/TCD65/Build+Script+Interaction+with+TeamCity#BuildScriptInteractionwithTeamCity-PublishingArtifactswhiletheBuildisStillinProgress
    /// </summary>
    public class TeamCityArtifactPublisher : Task
    {
        /// <summary>
        /// The path to the folder or file that is to be published by teamcity
        /// </summary>
        [Dependency] public Task<string> ArtifactPath;

        /// <summary>
        /// The text writer that is used to output the teamcity publish command.
        /// Defaults to Console.Out.
        /// </summary>
        public TextWriter Output;

        public TeamCityArtifactPublisher()
        {
            Output = Console.Out;
        }

        public override void Build(IBounce bounce)
        {
            Output.WriteLine(@"##teamcity[publishArtifacts '{0}']", ArtifactPath.Value);
        }
    }
}