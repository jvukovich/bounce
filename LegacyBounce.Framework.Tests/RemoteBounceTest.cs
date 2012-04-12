using System.Collections.Generic;
using Moq;
using NUnit.Framework;

namespace LegacyBounce.Framework.Tests {
    [TestFixture]
    public class RemoteBounceTest {
        [Test]
        public void ShouldReturnRemoteTargetsAsWellAsLocalTargets() {
            var remote = new RemoteBounce();
            var remoteOne = new Mock<IObsoleteTask>().Object;
            var localOne = new Mock<IObsoleteTask>().Object;

            remote.ArgumentsForTargets(new { RemoteOne = remoteOne });

            var withRemoteTargets = (IDictionary<string, IObsoleteTask>) remote.WithRemoteTargets(new {LocalOne = localOne});

            Assert.That(withRemoteTargets["LocalOne"], Is.SameAs(localOne));
            Assert.That(withRemoteTargets["RemoteOne"], Is.SameAs(remoteOne));
            Assert.That(withRemoteTargets.Count, Is.EqualTo(2));
        }
    }
}