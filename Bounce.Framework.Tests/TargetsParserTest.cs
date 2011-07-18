using System.Collections.Generic;
using Moq;
using NUnit.Framework;

namespace Bounce.Framework.Tests {
    [TestFixture]
    public class TargetsParserTest {
        [Test]
        public void ShouldParseTargetsDictionaryFromDictionary() {
            IDictionary<string, ITask> dictionary = new Mock<IDictionary<string, ITask>>().Object;

            Assert.That(new TargetsParser().ParseTargetsFromObject(dictionary), Is.SameAs(dictionary));
        }

        [Test]
        public void ShouldParseTargetsFromObject() {
            var a = new Mock<ITask>().Object;
            var b = new Mock<ITask>().Object;

            var targets = new {
                A = a,
                B = b,
            };

            var targetsDictionary = new TargetsParser().ParseTargetsFromObject(targets);
            Assert.That(targetsDictionary.Count, Is.EqualTo(2));
            Assert.That(targetsDictionary["A"], Is.SameAs(a));
            Assert.That(targetsDictionary["B"], Is.SameAs(b));
        }

        [Test]
        public void ShouldCheckForNullIfPropertyAbsent() {
            // arrange

            var targets = new DummyTarget {
                A = new Mock<ITask>().Object,
                B = null
            };

            // act / assert (no exceptions)
            var targetsDictionary = new TargetsParser().ParseTargetsFromObject(targets);
        }

        private class DummyTarget {
            public ITask A { get; set; }
            public ITask B { get; set; }
        }
    }
}