using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Moq;

namespace Bounce.Framework.Tests {
    [TestFixture]
    public class TargetsBuilderTests {
        [Test]
        public void BuildTargetsShouldSkipNullTargets() {
            // arrange
            var bounceMock = new Mock<ITargetBuilderBounce>();
            var commandMock = new Mock<IBounceCommand>();

            bounceMock.Setup(_ => _.TaskScope(It.IsAny<ITask>(), commandMock.Object, It.IsAny<string>())).Returns(new Mock<ITaskScope>().Object);
            bounceMock.Setup(_ => _.Invoke(commandMock.Object, It.IsAny<ITask>())).Callback((IBounceCommand c, ITask t) => {
                if (t == null) {
                    throw new NullReferenceException();
                }
            });

            var targets = new List<Target> { new Target { Name = "OK", Task = new Mock<ITask>().Object }, new Target { Name = "Fail", Task = null } };
            var targetsBuilder = new TargetsBuilder();

            // act / assert (no exception)
            targetsBuilder.BuildTargets(bounceMock.Object, targets, commandMock.Object);
        }
    }
}
