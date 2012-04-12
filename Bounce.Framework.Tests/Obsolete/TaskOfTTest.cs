using System.IO;
using Bounce.Framework.Obsolete;
using NUnit.Framework;

namespace Bounce.Framework.Tests.Obsolete {
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