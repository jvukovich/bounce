using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;

namespace LegacyBounce.Framework.Tests {
    [TestFixture]
    public class TaskTest {
        [Test]
        public void ShouldReturnDependenciesMarkedWithAttribute() {
            IObsoleteTask dep = new Mock<IObsoleteTask>().Object;
            var task = new ATask {A = dep};

            Assert.That(task.Dependencies.Select(d => d.Task), Is.EquivalentTo(new[] {dep}));
        }

        class ATask : Task {
            [Dependency] public IObsoleteTask A;
        }

        [Test]
        public void NUnitTestShouldDependOnDlls() {
            Task<IEnumerable<string>> paths = new string [] {"one", "two"};
            var tests = new NUnitTests {DllPaths = paths};

            Assert.That(tests.Dependencies.Select(d => d.Task), Has.Member(paths));
        }

        [Test]
        public void InvokeShouldCallBuildWithBuildCommand()
        {
            var task = new BuildCleanTask();
            IBounce bounce = new Mock<IBounce>().Object;

            task.Invoke(new BounceCommandParser().Build, bounce);

            Assert.That(task.BuiltWithBounce, Is.SameAs(bounce));
        }

        [Test]
        public void InvokeShouldCallCleanWithCleanCommand()
        {
            var task = new BuildCleanTask();
            IBounce bounce = new Mock<IBounce>().Object;

            task.Invoke(new BounceCommandParser().Clean, bounce);

            Assert.That(task.CleanedWithBounce, Is.SameAs(bounce));
        }

        class BuildCleanTask : Task
        {
            public IBounce BuiltWithBounce;
            public IBounce CleanedWithBounce;

            public override void Build(IBounce bounce) {
                BuiltWithBounce = bounce;
            }

            public override void Clean(IBounce bounce) {
                CleanedWithBounce = bounce;
            }
        }
    }
}