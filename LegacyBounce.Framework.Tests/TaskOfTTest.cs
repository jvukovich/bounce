using System.IO;
using NUnit.Framework;

namespace LegacyBounce.Framework.Tests {
    [TestFixture]
    public class TaskOfTTest {
        [Test, Ignore]
        public void ShouldFailIfFutureIsNotDependency() {
            var task = new TaskWithFutureNotDependency(new StringWriter()) {Description = "one"};

            Assert.That(() => task.TestBuild(), Throws.InstanceOf(typeof (DependencyBuildFailureException)));
        }

        class TaskWithFutureNotDependency : Task {
            public Task<string> Description;
            private StringWriter Output;

            public TaskWithFutureNotDependency(StringWriter output) {
                Output = output;
            }

            public override void Build() {
                Output.WriteLine(Description.Value);
            }
        }
    }
}