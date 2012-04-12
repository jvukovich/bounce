using System.IO;
using NUnit.Framework;

namespace Bounce.Framework.Tests.Obsolete {
    [TestFixture]
    public class BounceTest {
        [Test]
        public void ShouldReturnNullWriterIfLogOptionsAreNotDescribeTask() {
            var stdout = new StringWriter();
            var b = new Framework.Obsolete.Bounce();

            b.LogOptions.DescribeTasks = false;
            Assert.That(b.DescriptionOutput, Is.SameAs(TextWriter.Null));
        }

        [Test]
        public void ShouldReturnStdOutIfLogOptionsAreDescribeTask() {
            var stdout = new StringWriter();
            var b = new Framework.Obsolete.Bounce();

            b.LogOptions.DescribeTasks = true;
            b.LogOptions.StdOut = stdout;
            Assert.That(b.DescriptionOutput, Is.SameAs(stdout));
        }
    }
}