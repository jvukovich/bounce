using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Moq;

namespace Bounce.Framework.Tests {
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
            var bounce = factory.GetBounce(logFactoryMock.Object, new LogOptions());

            // assert
            Assert.That(bounce.LogFactory, Is.Not.Null);
            Assert.That(bounce.LogFactory, Is.TypeOf(logFactoryMock.Object.GetType()));
        }
    }
}
