using NUnit.Framework;

namespace LegacyBounce.Framework.Tests {
    [TestFixture]
    public class DirectoryFilesTest {
        [Test]
        public void ShouldReturnSubPathWhenBuilt() {
            Task<string> rootDir = "root";
            var df = new DirectoryFiles(rootDir);

            Task<string> subdir = df["subdir"];

            subdir.TestBuild();

            Assert.That(subdir.Value, Is.EqualTo(@"root\subdir"));
        }
    }
}