using System.IO;
using NUnit.Framework;

namespace Bounce.Framework.Tests {
    [TestFixture]
    public class BounceTest {
        [Test]
        public void ShouldReturnNullWriterIfLogOptionsAreNotDescribeTask() {
            var stdout = new StringWriter();
            var b = new Bounce(stdout, new StringWriter());

            b.LogOptions.DescribeTasks = false;
            Assert.That(b.DescriptionOutput, Is.SameAs(TextWriter.Null));
        }

        [Test]
        public void ShouldReturnStdOutIfLogOptionsAreDescribeTask() {
            var stdout = new StringWriter();
            var b = new Bounce(stdout, new StringWriter());

            b.LogOptions.DescribeTasks = true;
            Assert.That(b.DescriptionOutput, Is.SameAs(stdout));
        }
    }
}