using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace Bounce.Framework.Tests.Features {
    [TestFixture]
    public class DescribeFeature {
        [SetUp]
        public void SetUp() {
            Output = new StringWriter();
        }

        [Test]
        public void ShouldBuildMoreThanOneTarget() {
            MethodInfo method = typeof (TargetsProvider).GetMethod("GetTargets");
            new BounceRunner().Run(new[] {"describe", "One", "Two", "Three", "/env:test"}, method);

            Assert.That(Output.ToString(), Is.EqualTo("one\r\ntwo\r\n"));
        }

        [Test]
        public void WalkDescriptions() {
            var walker = new TaskWalker();

            var all = new All(new PrintTask(), new PrintTask());

            var ta = new TaskAggregator(dep => new TaskAggregate(dep));
            walker.Walk(new TaskDependency(all), ta.Before, ta.After);
        }

        class TaskAggregate : ITaskAggregate {
            private int Count = 0;
            private TaskDependency TaskDependency;

            public TaskAggregate(TaskDependency taskDependency) {
                TaskDependency = taskDependency;
            }

            public void Add(TaskDependency dep) {
                Count++;
            }

            public void Finally() {
                Console.WriteLine("task {0} has {1} deps", TaskDependency.Task.SmallDescription, Count);
            }
        }

        private void WriteStack() {
            Console.WriteLine("-----");
            foreach (var box in Stack) {
                Console.WriteLine(box.Description);
                Console.WriteLine("  " + String.Join("+", box.Tasks.ToArray()));
            }
            Console.WriteLine();
        }

        private void After(TaskDependency obj) {
            WriteStack();
            Stack.Pop();
        }

        private void Before(TaskDependency obj) {
            if (Stack.Count > 0) {
                Stack.Peek().Tasks.Add(obj.Task.SmallDescription);
            }
            Stack.Push(new Box() {Description = obj.Task.SmallDescription});
        }

        private Stack<Box> Stack = new Stack<Box>();

        class Box {
            public string Description = "";
            public List<string> Tasks = new List<string>();
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