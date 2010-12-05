using System.Collections.Generic;
using Moq;
using NUnit.Framework;

namespace Bounce.Framework.Tests {
    [TestFixture]
    public class RemoteBounceTest {
        [Test]
        public void ShouldReturnRemoteTargetsAsWellAsLocalTargets() {
            var remote = new RemoteBounce();
            var remoteOne = new Mock<ITask>().Object;
            var remoteTwo = new Mock<ITask>().Object;
            var localOne = new Mock<ITask>().Object;

            remote.Targets(new {RemoteOne = remoteOne, RemoteTwo = remoteTwo});

            var withRemoteTargets = (IDictionary<string, ITask>) remote.WithRemoteTargets(new {LocalOne = localOne});

            Assert.That(withRemoteTargets["LocalOne"], Is.SameAs(localOne));
            Assert.That(withRemoteTargets["RemoteOne"], Is.SameAs(remoteOne));
            Assert.That(withRemoteTargets["RemoteTwo"], Is.SameAs(remoteTwo));
            Assert.That(withRemoteTargets.Count, Is.EqualTo(3));
        }
    }
}