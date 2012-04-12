using System.Reflection;
using Bounce.Framework.Obsolete;
using Moq;
using NUnit.Framework;

namespace Bounce.Framework.Tests.Obsolete {
    [TestFixture]
    public class TargetsMethodInvokerTest {
        [Test]
        public void ShouldReturnTargetsFromMethodWithNoParameters() {
            var targets = new object();

            TargetsProvider.Targets = targets;

            MethodInfo targetsMethod = typeof(TargetsProvider).GetMethod("GetTargets");
            var actualTargets = new TargetsMethodInvoker().InvokeTargetsMethod(targetsMethod, null);

            Assert.That(actualTargets, Is.SameAs(targets));
        }

        [Test]
        public void ShouldReturnTargetsFromMethodWithParameters() {
            var targets = new object();

            TargetsProviderWithParameters.Targets = targets;
            var parameters = new Mock<IParameters>().Object;
            TargetsProviderWithParameters.ExpectedParameters = parameters;

            MethodInfo targetsMethod = typeof(TargetsProviderWithParameters).GetMethod("GetTargets");
            var actualTargets = new TargetsMethodInvoker().InvokeTargetsMethod(targetsMethod, parameters);

            Assert.That(actualTargets, Is.SameAs(targets));
        }

        class TargetsProvider {
            public static object Targets;

            public static object GetTargets() {
                return Targets;
            }
        }

        class TargetsProviderWithParameters {
            public static object Targets;
            public static IParameters ExpectedParameters;

            public static object GetTargets(IParameters p) {
                Assert.That(p, Is.SameAs(ExpectedParameters));
                return Targets;
            }
        }
    }
}