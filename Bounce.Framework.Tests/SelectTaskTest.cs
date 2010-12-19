using System.IO;
using NUnit.Framework;

namespace Bounce.Framework.Tests {
    [TestFixture]
    public class SelectTaskTest {
        [Test]
        public void ShouldReturnTaskThatIsBuiltFromFuture() {
            Future<int> fn = 10;

            var output = new StringWriter();
            var printer = fn.SelectTask(n => new FakePrintTask(output, n.ToString()));

            printer.TestBuild();

            Assert.That(output.ToString(), Is.EqualTo("10;"));
        }
    }
}