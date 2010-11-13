using System.Linq;
using Moq;
using NUnit.Framework;

namespace Bounce.Framework.Tests {
    [TestFixture]
    public class TaskTest {
        [Test]
        public void ShouldReturnDependenciesMarkedWithAttribute() {
            ITask dep = new Mock<ITask>().Object;
            var task = new ATask {A = dep};

            Assert.That(task.Dependencies, Is.EquivalentTo(new[] {dep}));
        }

        class ATask : Task {
            [Dependency] public ITask A;
        }

        [Test]
        public void NUnitTestShouldDependOnDlls() {
            var paths = new Val<string> [] {"one", "two"};
            var tests = new NUnitTests {DllPaths = paths};
            
            Assert.That(tests.Dependencies.ToArray(), Has.Member(paths[0]).And.Member(paths[1]));
        }
    }
}