using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Bounce.Framework.Obsolete;
using NUnit.Framework;

namespace Bounce.Framework.Tests.Obsolete.Features {
    [TestFixture]
    public class DescribeFeature {
        [SetUp]
        public void SetUp() {
            Output = new StringWriter();
        }

        [Test, Ignore] // not finished yet!
        public void ShouldBuildMoreThanOneTarget() {
            MethodInfo method = typeof (TargetsProvider).GetMethod("GetTargets");
            new BounceRunner().Run(new[] {"describe", "One", "Two", "Three", "/env:test"}, method);

            Assert.That(Output.ToString(), Is.EqualTo("one\r\ntwo\r\n"));
        }

        [Test]
        public void WalkDescriptions() {
            var walker = new TaskWalker();

            var all = new All(new PrintTask(), new PrintTask());

            var ta = new TaskAggregator<TaskCount>(dep => new TaskCount(dep));
            walker.Walk(new TaskDependency(all), ta.Before, ta.After);
            Assert.That(ta.Aggregate.Count, Is.EqualTo(3));
        }

        [Test]
        public void WalkAggregatesButNoLoopingForever() {
            var walker = new TaskWalker();

            var all = new All();
            var allDeps = new List<ITask> {new PrintTask(), new PrintTask(), all};
            all.Tasks = allDeps;

            var ta = new TaskAggregator<TaskCount>(dep => new TaskCount(dep));
            walker.Walk(new TaskDependency(all), ta.Before, ta.After);
            Assert.That(ta.Aggregate.Count, Is.EqualTo(3));
        }

        [Test]
        public void PurityAggregateReturnsPureIfAllSubTasksArePure() {
            var walker = new TaskWalker();

            var all = new All(new PureTask(), new PureTask());

            var ta = new TaskAggregator<TaskPurity>(dep => new TaskPurity(dep));
            walker.Walk(new TaskDependency(all), ta.Before, ta.After);
            Assert.That(ta.Aggregate.Purity, Is.EqualTo(true));
        }

        [Test]
        public void PurityAggregateReturnsImpureIfAnySubTasksAreImpure() {
            var walker = new TaskWalker();

            var all = new All(new ImpureTask(), new PureTask());

            var ta = new TaskAggregator<TaskPurity>(dep => new TaskPurity(dep));
            walker.Walk(new TaskDependency(all), ta.Before, ta.After);
            Assert.That(ta.Aggregate.Purity, Is.EqualTo(false));
        }

        public class PureTask : FakeTask {
            public override bool IsPure { get { return true; } }
        }

        public class ImpureTask : FakeTask {
            public override bool IsPure { get { return false; } }
        }

        class TaskCount : ITaskAggregate<TaskCount> {
            public int Count = 1;
            private TaskDependency TaskDependency;

            public TaskCount(TaskDependency taskDependency) {
                TaskDependency = taskDependency;
            }

            public void Add(TaskCount agg) {
                Count += agg.Count;
            }

            public void Finally() {
            }
        }

        private static TextWriter Output;

        class TargetsProvider {
            private static PrintTask Print(Task<string> msg) {
                return new PrintTask(Output) {Description = msg};
            }

            [Targets]
            public static object GetTargets(IParameters parameters) {
                var sol = new VisualStudioSolution {SolutionPath = "stuff.sln"};

                var env = parameters.Required<string>("env").Switch(
                    "test", Print(sol.Projects["cats"].OutputFile),
                    "stage", Print("this is test"),
                    "live", Print("this is test"));

                var one = new PrintTask(Output) { Description = "*------------*one" };
                var two = new PrintTask(Output) { Description = "*------------*two" };

                return new {
                    One = one,
                    Two = two,
                    Three = env,
                };
            }
        }
    }
}