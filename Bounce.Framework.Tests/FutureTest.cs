using System.IO;
using NUnit.Framework;

namespace Bounce.Framework.Tests {
    [TestFixture]
    public class FutureTest {
        [Test, Ignore]
        public void ShouldFailIfFutureIsNotDependency() {
            var task = new TaskWithFutureNotDependency(new StringWriter()) {Description = "one"};

            Assert.That(() => task.TestBuild(), Throws.InstanceOf(typeof (DependencyBuildFailureException)));
        }

        class TaskWithFutureNotDependency : Task {
            public Future<string> Description;
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