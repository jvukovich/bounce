using Bounce.Framework.Obsolete;
using NUnit.Framework;

namespace Bounce.Framework.Tests.Obsolete {
    [TestFixture]
    public class WhenBuiltTest {
        [Test]
        public void ShouldBuildDependency() {
            var task = new DependencyTask();

            var one = task.WhenBuilt(() => "one");

            one.TestBuild();

            Assert.That(one.Value, Is.EqualTo("one"));
            Assert.That(task.WasBuilt);
        }

        class DependencyTask : Task {
            public bool WasBuilt;

            public override void Build(IBounce bounce) {
                WasBuilt = true;
            }
        }
    }
}