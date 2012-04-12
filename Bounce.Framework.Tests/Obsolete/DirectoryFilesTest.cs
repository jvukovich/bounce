using Bounce.Framework.Obsolete;
using NUnit.Framework;

namespace Bounce.Framework.Tests.Obsolete {
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