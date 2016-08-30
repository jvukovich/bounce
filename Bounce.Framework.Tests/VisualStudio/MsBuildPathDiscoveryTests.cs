using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using Bounce.Framework.VisualStudio;
using Moq;
using NUnit.Framework;

namespace Bounce.Framework.Tests.VisualStudio
{
    [TestFixture]
    class MsBuildPathDiscoveryTests {
        private MsBuildPathDiscovery Discoverer;
        private Mock<IFileSystemWrapper> FsMock;
        private List<string> FileSystemContents;

        [SetUp]
        public void SetUp() {
            FileSystemContents = new List<string>();

            FsMock = new Mock<IFileSystemWrapper>();
            FsMock.Setup(x => x.FileExists(It.IsAny<string>())).Returns((string s) => FileSystemContents.Contains(s));
            FsMock.Setup(x => x.DirectoryExists(It.IsAny<string>())).Returns((string s) => FileSystemContents.Contains(s));

            Discoverer = new MsBuildPathDiscovery(FsMock.Object);
            Discoverer.MsBuildLocations.Clear();
            Discoverer.MsBuildLocations.Add("c:\\msbuild.exe");
            Discoverer.MsBuildLocations.Add("c:\\other\\place\\msbuild.exe");
        }

        [Test]
        public void FindsLatestMsBuildWhenExistsCheckPasses() {
            FileSystemContents.Add("c:\\msbuild.exe");

            string path = Discoverer.LocateMostRecentMsBuildPath();

            Assert.That(path, Is.EqualTo("c:\\msbuild.exe"));
        }

        [Test]
        public void ReturnsFallbackForBackwardsCompatabilityIfItCantFindAnything() {
            FileSystemContents.Clear();

            string path = Discoverer.LocateMostRecentMsBuildPath();

            Assert.That(path, Is.EqualTo("c:\\other\\place\\msbuild.exe"));
        }

        [Test]
        public void MsBuildLocationsPopulatedWithEnvVariableIfAvailable() {
            Environment.SetEnvironmentVariable("MSBuild", "c:\\msbuild.exe");

            Assert.That(Discoverer.MsBuildLocations[0], Is.EqualTo("c:\\msbuild.exe"));
        }

        [Test]
        public void MsBuildLocationsFiltersOutNoneExistentPlaceholder() {
            FileSystemContents.Add("c:\\msbuild.exe");

            Assert.That(Discoverer.MsBuildLocations[0], Is.Not.EqualTo("%MSBuild%"));
        }

        [Test]
        public void ReturnsScannedLocationsWithResult() {
            Discoverer = new MsBuildPathDiscovery(FsMock.Object);

            var path = Discoverer.LocateMostRecentMsBuildPath();

            Assert.That(path.SearchLocations, Is.Not.Empty);
        }

        [Test]
        public void LookupLocationsTracked() {
            Discoverer = new MsBuildPathDiscovery(FsMock.Object);
            FileSystemContents.Add("C:\\Program Files (x86)\\MSBuild\\14.0\\Bin\\msbuild.exe");

            var path = Discoverer.LocateMostRecentMsBuildPath();

            Assert.That(path.SearchLocations, Is.Not.Empty);
        }
    }
}
