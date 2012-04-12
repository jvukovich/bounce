using Bounce.Framework.Obsolete;
using Moq;
using NUnit.Framework;

namespace Bounce.Framework.Tests.Obsolete {
    [TestFixture]
    public class LogFactoryRegistryTest {
        [Test]
        public void ShouldReturnNameOfLogFactory() {
            var logFactoryRegistry = new LogFactoryRegistry();
            var mylogger = new Mock<ITaskLogFactory>().Object;

            logFactoryRegistry.RegisterLogFactory("mylogger", mylogger);
            Assert.That(logFactoryRegistry.FindNameForLogFactory(mylogger), Is.EqualTo("mylogger"));
        }

        [Test]
        public void ShouldReturnNullIfLogFactoryNotFound() {
            var logFactoryRegistry = new LogFactoryRegistry();
            var mylogger = new Mock<ITaskLogFactory>().Object;

            Assert.That(logFactoryRegistry.FindNameForLogFactory(mylogger), Is.Null);
        }

        [Test]
        public void ShouldReturnLogFactoryByName() {
            var logFactoryRegistry = new LogFactoryRegistry();
            var mylogger = new Mock<ITaskLogFactory>().Object;

            logFactoryRegistry.RegisterLogFactory("mylogger", mylogger);
            Assert.That(logFactoryRegistry.GetLogFactoryByName("mylogger"), Is.SameAs(mylogger));
        }

        [Test]
        public void ThrowsExceptionIfLoggerNotFoundByName() {
            var logFactoryRegistry = new LogFactoryRegistry();

            Assert.That(() => logFactoryRegistry.GetLogFactoryByName("mylogger"),
                        Throws.InstanceOf(typeof (ConfigurationException)));
        }
    }
}