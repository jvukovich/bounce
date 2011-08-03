﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using ICSharpCode.SharpZipLib.Zip;
using NUnit.Framework;
using Moq;
using SharpTestsEx;

namespace Bounce.Framework.Tests {
    [TestFixture]
    public class VisualStudioSolutionTest {
        private const string SolutionUnzipDirectory = "TestSolution";

        [Test]
        public void BuildsOutputFileOfFirstProject() {
            // arrange
            var bounceMock = new Mock<IBounce>() { DefaultValue = Moq.DefaultValue.Mock };
            var shellMock = new Mock<IShellCommandExecutor>();
            var solution = new VisualStudioSolution { SolutionPath = new ImmediateValue<string>(@"TestSolution.sln") };

            bounceMock.SetupAllProperties();
            bounceMock.SetupGet(_ => _.ShellCommand).Returns(shellMock.Object);

            // act
            solution.Build(bounceMock.Object);

            // assert
            shellMock.Verify(_ => _.ExecuteAndExpectSuccess(It.IsAny<string>(), It.Is<string>(s => s.Satisfy(v => v == "\"TestSolution.sln\""))));
        }

        [Test]
        public void CanAccessProjectsBeforeSolutionExists() {
            // arrange 
            var solution = new VisualStudioSolution {SolutionPath = new ImmediateValue<string>(Path.Combine(SolutionUnzipDirectory, @"TestSolution\TestSolution.sln"))};

            // act 
            Task<IEnumerable<string>> outputFiles = solution.Projects.Select(p => p.OutputFile);

            // assert
            Assert.That(outputFiles.Value.ToArray(), Is.EquivalentTo(new [] {@"TestSolution\TestSolution\TestSolution\bin\Debug\TestSolution.dll"}));
        }

        [Test]
        public void BuildIfConfigurationIsSet() {
            // arrange
            var bounceMock = new Mock<IBounce>() { DefaultValue = Moq.DefaultValue.Mock };
            var shellMock = new Mock<IShellCommandExecutor>();
            var solution = new VisualStudioSolution { SolutionPath = new ImmediateValue<string>(@"TestSolution.sln"), Configuration = "Release" };

            bounceMock.SetupAllProperties();
            bounceMock.SetupGet(_ => _.ShellCommand).Returns(shellMock.Object);

            // act
            solution.Build(bounceMock.Object);

            // assert
            shellMock.Verify(_ => _.ExecuteAndExpectSuccess(It.IsAny<string>(), It.Is<string>(s => s.Satisfy(v => v == "\"TestSolution.sln\" /p:Configuration=Release"))));
        }

        [Test]
        public void BuildIfConfigurationIsSetToNull() {
            // arrange
            var bounceMock = new Mock<IBounce>() { DefaultValue = Moq.DefaultValue.Mock };
            var shellMock = new Mock<IShellCommandExecutor>();
            var solution = new VisualStudioSolution { SolutionPath = new ImmediateValue<string>(@"TestSolution.sln"), Configuration = null };

            bounceMock.SetupAllProperties();
            bounceMock.SetupGet(_ => _.ShellCommand).Returns(shellMock.Object);

            // act
            solution.Build(bounceMock.Object);

            // assert
            shellMock.Verify(_ => _.ExecuteAndExpectSuccess(It.IsAny<string>(), It.Is<string>(s => s.Satisfy(v => v == "\"TestSolution.sln\""))));
        }

        [Test]
        public void BuildIfConfigurationValueIsSetToNull() {
            // arrange
            var bounceMock = new Mock<IBounce>() { DefaultValue = Moq.DefaultValue.Mock };
            var shellMock = new Mock<IShellCommandExecutor>();
            var solution = new VisualStudioSolution { SolutionPath = new ImmediateValue<string>(@"TestSolution.sln"), Configuration = new ImmediateValue<string>(null) };

            bounceMock.SetupAllProperties();
            bounceMock.SetupGet(_ => _.ShellCommand).Returns(shellMock.Object);

            // act
            solution.Build(bounceMock.Object);

            // assert
            shellMock.Verify(_ => _.ExecuteAndExpectSuccess(It.IsAny<string>(), It.Is<string>(s => s.Satisfy(v => v == "\"TestSolution.sln\""))));
        }

        [Test]
        public void BuildWithOutdirOption() {
            // arrange
            var bounceMock = new Mock<IBounce>() { DefaultValue = Moq.DefaultValue.Mock };
            var shellMock = new Mock<IShellCommandExecutor>();
            var solution = new VisualStudioSolution { SolutionPath = new ImmediateValue<string>(@"TestSolution.sln"), OutputDir = "..\\Build\\" };

            bounceMock.SetupAllProperties();
            bounceMock.SetupGet(_ => _.ShellCommand).Returns(shellMock.Object);

            // act
            solution.Build(bounceMock.Object);

            // assert
            shellMock.Verify(_ => _.ExecuteAndExpectSuccess(It.IsAny<string>(), It.Is<string>(s => s.Satisfy(v => v == "\"TestSolution.sln\" /p:Outdir=..\\Build\\"))));
        }

        [Test]
        public void BuildIfOutdirOptionIsNull() {
            // arrange
            var bounceMock = new Mock<IBounce>() { DefaultValue = Moq.DefaultValue.Mock };
            var shellMock = new Mock<IShellCommandExecutor>();
            var solution = new VisualStudioSolution { SolutionPath = new ImmediateValue<string>(@"TestSolution.sln"), OutputDir = null };

            bounceMock.SetupAllProperties();
            bounceMock.SetupGet(_ => _.ShellCommand).Returns(shellMock.Object);

            // act
            solution.Build(bounceMock.Object);

            // assert
            shellMock.Verify(_ => _.ExecuteAndExpectSuccess(It.IsAny<string>(), It.Is<string>(s => s.Satisfy(v => v == "\"TestSolution.sln\""))));
        }

        [Test]
        public void BuildIfOutdirOptionValueIsNull() {
            // arrange
            var bounceMock = new Mock<IBounce>() { DefaultValue = Moq.DefaultValue.Mock };
            var shellMock = new Mock<IShellCommandExecutor>();
            var solution = new VisualStudioSolution { SolutionPath = new ImmediateValue<string>(@"TestSolution.sln"), OutputDir = new ImmediateValue<string>(null) };

            bounceMock.SetupAllProperties();
            bounceMock.SetupGet(_ => _.ShellCommand).Returns(shellMock.Object);

            // act
            solution.Build(bounceMock.Object);

            // assert
            shellMock.Verify(_ => _.ExecuteAndExpectSuccess(It.IsAny<string>(), It.Is<string>(s => s.Satisfy(v => v == "\"TestSolution.sln\""))));
        }

        [Test]
        public void BuildWithOutdirOptionEnsureThatLastSlashIsSet() {
            // arrange
            var bounceMock = new Mock<IBounce>() { DefaultValue = Moq.DefaultValue.Mock };
            var shellMock = new Mock<IShellCommandExecutor>();
            var solution = new VisualStudioSolution { SolutionPath = new ImmediateValue<string>(@"TestSolution.sln"), OutputDir = "..\\Build" };

            bounceMock.SetupAllProperties();
            bounceMock.SetupGet(_ => _.ShellCommand).Returns(shellMock.Object);

            // act
            solution.Build(bounceMock.Object);

            // assert
            shellMock.Verify(_ => _.ExecuteAndExpectSuccess(It.IsAny<string>(), It.Is<string>(s => s.Satisfy(v => v == "\"TestSolution.sln\" /p:Outdir=..\\Build\\"))));
        }

    }
}