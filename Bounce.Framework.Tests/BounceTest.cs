using System.IO;
using NUnit.Framework;

namespace Bounce.Framework.Tests {
    [TestFixture]
    public class BounceTest {
        [Test]
        public void ShouldReturnNullWriterIfLogOptionsAreNotDescribeTask() {
            var stdout = new StringWriter();
            var b = new Bounce(stdout, new StringWriter());

            b.LogOptions.DescribeTasks = false;
            Assert.That(b.DescriptionOutput, Is.SameAs(TextWriter.Null));
        }

        [Test]
        public void ShouldReturnStdOutIfLogOptionsAreDescribeTask() {
            var stdout = new StringWriter();
            var b = new Bounce(stdout, new StringWriter());

            b.LogOptions.DescribeTasks = true;
            Assert.That(b.DescriptionOutput, Is.SameAs(stdout));
        }
    }

    [TestFixture]
    public class ParameterTest {
        [Test]
        public void ShouldGenerateParameter() {
            var parameter = new Parameter<int>() {Name = "name", DefaultValue = 45};
            Assert.That(parameter.Generate(TypeParsers.Default), Is.EqualTo("/name:45"));
        }
    }

}