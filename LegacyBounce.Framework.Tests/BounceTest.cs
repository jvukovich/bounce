using System.IO;
using NUnit.Framework;

namespace LegacyBounce.Framework.Tests {
    [TestFixture]
    public class BounceTest {
        [Test]
        public void ShouldReturnNullWriterIfLogOptionsAreNotDescribeTask() {
            var stdout = new StringWriter();
            var b = new Bounce();

            b.LogOptions.DescribeTasks = false;
            Assert.That(b.DescriptionOutput, Is.SameAs(TextWriter.Null));
        }

        [Test]
        public void ShouldReturnStdOutIfLogOptionsAreDescribeTask() {
            var stdout = new StringWriter();
            var b = new Bounce();

            b.LogOptions.DescribeTasks = true;
            b.LogOptions.StdOut = stdout;
            Assert.That(b.DescriptionOutput, Is.SameAs(stdout));
        }
    }
}