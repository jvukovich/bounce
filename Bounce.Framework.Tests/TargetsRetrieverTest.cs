using System.Collections.Generic;
using System.Reflection;
using Moq;
using NUnit.Framework;

namespace Bounce.Framework.Tests {
    [TestFixture]
    public class TargetsRetrieverTest {
        [Test]
        public void ShouldReturnTargetsFromTargetsMethod() {
            var a = new Mock<ITask>().Object;
            var b = new Mock<ITask>().Object;
            TargetsProvider.Targets = new {A = a, B = b};

            var targetsRetriever = new TargetsRetriever();
            MethodInfo targetsMethod = typeof (TargetsProvider).GetMethod("GetTargets");

            var targetsDictionary = targetsRetriever.GetTargetsFromAssembly(targetsMethod, null);

            Assert.That(targetsDictionary.Count, Is.EqualTo(2));
            Assert.That(targetsDictionary["A"], Is.SameAs(a));
            Assert.That(targetsDictionary["B"], Is.SameAs(b));
        }

        class TargetsProvider {
            public static object Targets;

            public static object GetTargets() {
                return Targets;
            }
        }
    }
}