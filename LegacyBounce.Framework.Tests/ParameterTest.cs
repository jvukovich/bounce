using NUnit.Framework;

namespace LegacyBounce.Framework.Tests {
    [TestFixture]
    public class ParameterTest {
        [Test]
        public void ShouldGenerateParameter() {
            var parameter = new Parameter<int>() {Name = "name", DefaultValue = 45};
            Assert.That(parameter.Generate(TypeParsers.Default), Is.EqualTo("/name:45"));
        }

        [Test]
        public void ShouldGenerateParameterWithQuotesIfContainsSpaces() {
            var parameter = new Parameter<string>() {Name = "name", DefaultValue = "one two"};
            Assert.That(parameter.Generate(TypeParsers.Default), Is.EqualTo(@"""/name:one two"""));
        }
    }
}