using System.Collections.Generic;
using Bounce.Framework.Obsolete;
using Moq;
using NUnit.Framework;

namespace Bounce.Framework.Tests.Obsolete {
    [TestFixture]
    public class TargetsParserTest {
        [Test]
        public void ShouldParseTargetsDictionaryFromDictionary() {
            IDictionary<string, IObsoleteTask> dictionary = new Mock<IDictionary<string, IObsoleteTask>>().Object;

            Assert.That(new TargetsParser().ParseTargetsFromObject(dictionary), Is.SameAs(dictionary));
        }

        [Test]
        public void ShouldParseTargetsFromObject() {
            var a = new Mock<IObsoleteTask>().Object;
            var b = new Mock<IObsoleteTask>().Object;

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
                A = new Mock<IObsoleteTask>().Object,
                B = null
            };

            // act / assert (no exceptions)
            var targetsDictionary = new TargetsParser().ParseTargetsFromObject(targets);
        }

        private class DummyTarget {
            public IObsoleteTask A { get; set; }
            public IObsoleteTask B { get; set; }
        }
    }
}