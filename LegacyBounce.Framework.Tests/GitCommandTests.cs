using System.Collections.Generic;
using Moq;
using NUnit.Framework;

namespace LegacyBounce.Framework.Tests {
    [TestFixture]
    public class GitCommandTests {
        [Test]
        public void Clone() {
            // arrange
            var logMock = new Mock<ILog>();
            var bounceMock = new Mock<IBounce>();
            var shellCommandMock = new Mock<IShellCommandExecutor>();
            var gitCommand = new GitCommand();

            bounceMock.Setup(_ => _.ShellCommand).Returns(shellCommandMock.Object);

            // act 
            gitCommand.Clone("git@github.com:alexanderbeletsky/bounce.git", "currentDirectory", null, logMock.Object, bounceMock.Object);

            // assert
            shellCommandMock.Verify(_ => _.ExecuteAndExpectSuccess("cmd" , "/C git clone git@github.com:alexanderbeletsky/bounce.git \"currentDirectory\""));
        }

        [Test]
        public void Pull() {
            // arrange
            var logMock = new Mock<ILog>();
            var bounceMock = new Mock<IBounce>();
            var shellCommandMock = new Mock<IShellCommandExecutor>();
            var gitCommand = new GitCommand();

            bounceMock.Setup(_ => _.ShellCommand).Returns(shellCommandMock.Object);

            // act
            gitCommand.Pull(".", logMock.Object, bounceMock.Object);
            
            // assert
            shellCommandMock.Verify(_ => _.ExecuteAndExpectSuccess("cmd", "/C git pull"));
        }

        [Test]
        public void Tag_WithForceFlagTrue() {
            // arrange
            var logMock = new Mock<ILog>();
            var bounceMock = new Mock<IBounce>();
            var shellCommandMock = new Mock<IShellCommandExecutor>();
            var gitCommand = new GitCommand();

            bounceMock.Setup(_ => _.ShellCommand).Returns(shellCommandMock.Object);

            // act
            gitCommand.Tag("MyTag", true, bounceMock.Object);

            // assert
            shellCommandMock.Verify(_ => _.ExecuteAndExpectSuccess("cmd", "/C git tag -f MyTag"));
        }

        [Test]
        public void Tag_WithOutForceFlagTrue() {
            // arrange
            var logMock = new Mock<ILog>();
            var bounceMock = new Mock<IBounce>();
            var shellCommandMock = new Mock<IShellCommandExecutor>();
            var gitCommand = new GitCommand();

            bounceMock.Setup(_ => _.ShellCommand).Returns(shellCommandMock.Object);

            // act
            gitCommand.Tag("MyTag", false, bounceMock.Object);

            // assert
            shellCommandMock.Verify(_ => _.ExecuteAndExpectSuccess("cmd", "/C git tag MyTag"));
        }

        [Test]
        public void Clone_WithOptions() {
            // arrange
            var logMock = new Mock<ILog>();
            var bounceMock = new Mock<IBounce>();
            var shellCommandMock = new Mock<IShellCommandExecutor>();
            var gitCommand = new GitCommand();

            bounceMock.Setup(_ => _.ShellCommand).Returns(shellCommandMock.Object);

            var options = new Dictionary<string, string> { { "--branch", "develop" } };

            // act 
            gitCommand.Clone("git@github.com:alexanderbeletsky/bounce.git", "currentDirectory", options, logMock.Object, bounceMock.Object);

            // assert
            shellCommandMock.Verify(_ => _.ExecuteAndExpectSuccess("cmd", "/C git clone --branch develop git@github.com:alexanderbeletsky/bounce.git \"currentDirectory\""));
        }

        [Test]
        public void Clone_WithSeveralOptions() {
            // arrange
            var logMock = new Mock<ILog>();
            var bounceMock = new Mock<IBounce>();
            var shellCommandMock = new Mock<IShellCommandExecutor>();
            var gitCommand = new GitCommand();

            bounceMock.Setup(_ => _.ShellCommand).Returns(shellCommandMock.Object);

            var options = new Dictionary<string, string> { { "--branch", "develop" }, { "--origin", "myorig" } };

            // act 
            gitCommand.Clone("git@github.com:alexanderbeletsky/bounce.git", "currentDirectory", options, logMock.Object, bounceMock.Object);

            // assert
            shellCommandMock.Verify(_ => _.ExecuteAndExpectSuccess("cmd", "/C git clone --branch develop --origin myorig git@github.com:alexanderbeletsky/bounce.git \"currentDirectory\""));
        }
    }
}
