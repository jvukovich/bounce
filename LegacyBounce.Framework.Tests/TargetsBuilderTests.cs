using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;

namespace LegacyBounce.Framework.Tests {
    [TestFixture]
    public class TargetsBuilderTests {
        [Test]
        public void BuildTargetsShouldSkipNullTargets() {
            // arrange
            var bounceMock = new Mock<ITargetBuilderBounce>();
            var commandMock = new Mock<IBounceCommand>();

            bounceMock.Setup(_ => _.TaskScope(It.IsAny<IObsoleteTask>(), commandMock.Object, It.IsAny<string>())).Returns(new Mock<ITaskScope>().Object);
            bounceMock.Setup(_ => _.Invoke(commandMock.Object, It.IsAny<IObsoleteTask>())).Callback((IBounceCommand c, IObsoleteTask t) => {
                if (t == null) {
                    throw new NullReferenceException();
                }
            });

            var targets = new List<Target> { new Target { Name = "OK", Task = new Mock<IObsoleteTask>().Object }, new Target { Name = "Fail", Task = null } };
            var targetsBuilder = new TargetsBuilder();

            // act / assert (no exception)
            targetsBuilder.BuildTargets(bounceMock.Object, targets, commandMock.Object);
        }
    }
}
