using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using Bounce.Framework.VisualStudio;
using NUnit.Framework;

namespace Bounce.Framework.Tests.VisualStudio
{
    [TestFixture]
    class MsBuildPathDiscoveryTests {
        private MsBuildPathDiscovery Discoverer;

        [SetUp]
        public void SetUp() {
            Discoverer = new MsBuildPathDiscovery();
            Discoverer.MsBuildLocations.Clear();
            Discoverer.MsBuildLocations.Add("c:\\msbuild.exe");
            Discoverer.MsBuildLocations.Add("c:\\other\\place\\msbuild.exe");
        }

        [Test]
        public void FindsLatestMsBuildWhenExistsCheckPasses() {
            Discoverer.FileExitsCheck = s => true;

            var path = Discoverer.LocateMostRecentMsBuildPath();

            Assert.That(path, Is.EqualTo("c:\\msbuild.exe"));
        }

        [Test]
        public void ReturnsFallbackForBackwardsCompatabilityIfItCantFindAnything() {
            Discoverer.FileExitsCheck = s => false;

            var path = Discoverer.LocateMostRecentMsBuildPath();

            Assert.That(path, Is.EqualTo("c:\\other\\place\\msbuild.exe"));
        }

        [Test]
        public void MsBuildLocationsPopulatedWithEnvVariableIfAvailable() {
            Environment.SetEnvironmentVariable("MSBuild", "c:\\msbuild.exe");

            Discoverer = new MsBuildPathDiscovery();

            Assert.That(Discoverer.MsBuildLocations[0], Is.EqualTo("c:\\msbuild.exe"));
        }

        [Test]
        public void MsBuildLocationsFiltersOutNoneExistentPlaceholder() {
            Discoverer = new MsBuildPathDiscovery();

            Assert.That(Discoverer.MsBuildLocations[0], Is.Not.EqualTo("%MSBuild%"));
        }
    }
}
