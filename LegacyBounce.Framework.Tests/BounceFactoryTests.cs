using Moq;
using NUnit.Framework;

namespace LegacyBounce.Framework.Tests {
    [TestFixture]
    public class BounceFactoryTests {
        [Test]
        public void CreateDefaultInstance() {
            // arrange
            var factory = new BounceFactory();

            // act
            var bounce = factory.GetBounce();

            // assert
            Assert.That(bounce, Is.Not.Null);
        }

        [Test]
        public void CreateDefaultInstance_LogFactoryInitialized() {
            // arrange
            var factory = new BounceFactory();

            // act
            var bounce = factory.GetBounce();

            // assert
            Assert.That(bounce.LogFactory, Is.Not.Null);
        }

        [Test]
        public void CreateInstanceWithOverridenLogOptions() {
            // arrange
            var logFactoryMock = new Mock<ITaskLogFactory>();
            var factory = new BounceFactory();

            // act
            var logOptions = new LogOptions();
            var bounce = factory.GetBounce(logOptions);

            // assert
            Assert.That(bounce.LogOptions, Is.SameAs(logOptions));
        }
    }
}
