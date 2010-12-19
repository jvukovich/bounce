using System.Reflection;
using NUnit.Framework;

namespace Bounce.Framework.Tests.Features {
    [TestFixture]
    public class ShowingAvailableTargetsAndParametersFeature {
        [Test, Ignore("does an Application.Exit(1), so fails. Probably remove that so we can test this")]
        public void ShouldBuildMoreThanOneTarget() {
            MethodInfo method = typeof (TargetsProvider).GetMethod("GetTargets");
            new BounceRunner().Run(new string[0], method);
        }

        class TargetsProvider {
            [Targets]
            public static object GetTargets(IParameters parameters) {
                var one = new FakeTask {Dependencies = new [] {new TaskDependency {Task = parameters.Required<int>("port")}}};
                var two = new FakeTask {Dependencies = new [] {new TaskDependency {Task = parameters.Default("name", "Some Web Site")}}};

                return new {
                    One = one,
                    Two = two,
                };
            }
        }
    }
}