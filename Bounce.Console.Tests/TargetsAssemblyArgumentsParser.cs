using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using NUnit.Framework;

namespace Bounce.Console.Tests
{
    [TestFixture]
    public class TargetsAssemblyArgumentsParserTest
    {
        [Test]
        public void ShouldParseTargetsFromParameterWithColon() {
            var finder = new Mock<IBounceDirectoryFinder>();
            var parser = new TargetsAssemblyArgumentsParser(finder.Object);
            var targetsAndArgs = parser.GetTargetsAssembly(new[] {"/bounceDir:adir", "other", "args"});

            Assert.That(targetsAndArgs.RemainingArguments, Is.EqualTo(new[] {"other", "args"}));
            Assert.That(targetsAndArgs.BounceDirectory, Is.EqualTo("adir"));
        }

        [Test]
        public void ShouldParseTargetsFromParameter() {
            var finder = new Mock<IBounceDirectoryFinder>();
            var parser = new TargetsAssemblyArgumentsParser(finder.Object);
            var targetsAndArgs = parser.GetTargetsAssembly(new[] {"/bounceDir", "adir", "other", "args"});

            Assert.That(targetsAndArgs.RemainingArguments, Is.EqualTo(new[] {"other", "args"}));
            Assert.That(targetsAndArgs.BounceDirectory, Is.EqualTo("adir"));
        }

        [Test]
        public void ShouldParseTargetsAndRecurseFromArguments() {
            var finder = new Mock<IBounceDirectoryFinder>();
            var parser = new TargetsAssemblyArgumentsParser(finder.Object);
            var targetsAndArgs = parser.GetTargetsAssembly(new[] {"/bounceDir", "adir", "/recurse", "other", "args"});

            Assert.That(targetsAndArgs.RemainingArguments, Is.EqualTo(new[] {"other", "args"}));
            Assert.That(targetsAndArgs.BounceDirectory, Is.EqualTo("adir"));
            Assert.That(targetsAndArgs.Recurse);
        }

        [Test]
        public void ShouldParseRecurseFromArguments() {
            var finder = new Mock<IBounceDirectoryFinder>();
            finder.Setup(f => f.FindBounceDirectory()).Returns(@"path\to\bounce");

            var parser = new TargetsAssemblyArgumentsParser(finder.Object);
            var targetsAndArgs = parser.GetTargetsAssembly(new[] {"/recurse", "other", "args"});

            Assert.That(targetsAndArgs.Recurse);
            Assert.That(targetsAndArgs.RemainingArguments, Is.EqualTo(new[] { "other", "args" }));
            Assert.That(targetsAndArgs.BounceDirectory, Is.SameAs(@"path\to\bounce"));
        }

        [Test]
        public void ShouldFindTargetsIfNoTargetsParameterGiven() {
            var finder = new Mock<IBounceDirectoryFinder>();
            finder.Setup(f => f.FindBounceDirectory()).Returns(@"path\to\bounce");

            var parser = new TargetsAssemblyArgumentsParser(finder.Object);
            var targetsAndArgs = parser.GetTargetsAssembly(new[] {"build", "SomeTarget", "/other", "args"});

            Assert.That(targetsAndArgs.RemainingArguments, Is.EqualTo(new[] {"build", "SomeTarget", "/other", "args"}));
            Assert.That(targetsAndArgs.BounceDirectory, Is.SameAs(@"path\to\bounce"));
            Assert.That(targetsAndArgs.Recurse, Is.False);
        }

        [Test]
        public void ShouldThrowIfTargetsAssemblyNotFound() {
            var finder = new Mock<IBounceDirectoryFinder>();

            var parser = new TargetsAssemblyArgumentsParser(finder.Object);
            Assert.That(() => parser.GetTargetsAssembly(new[] {"SomeTask", "/other", "args"}), Throws.InstanceOf(typeof(TargetsAssemblyNotFoundException)));
        }

        [Test]
        public void ShouldAttemptToFindAssemblyIfNoArgsGiven() {
            var finder = new Mock<IBounceDirectoryFinder>();
            finder.Setup(f => f.FindBounceDirectory()).Returns(@"path\to\bounce");

            var parser = new TargetsAssemblyArgumentsParser(finder.Object);
            var targetsAndArgs = parser.GetTargetsAssembly(new string[0]);

            Assert.That(targetsAndArgs.RemainingArguments, Is.Empty);
            Assert.That(targetsAndArgs.BounceDirectory, Is.SameAs(@"path\to\bounce"));
        }
    }
}
