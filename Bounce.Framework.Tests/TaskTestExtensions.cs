using System;
using System.IO;
using System.Linq;
using Moq;

namespace Bounce.Framework.Tests {
    public static class TaskTestExtensions {
        public static void TestBuild(this ITask task) {
            TestInvoke(task, new BounceCommandParser().Build);
        }

        public static void TestClean(this ITask task) {
            TestInvoke(task, new BounceCommandParser().Clean);
        }

        public static void TestInvoke(this ITask task, IBounceCommand command) {
            new FakeTargetBuilderBounce().Invoke(command, task);
        }

        public class FakeTargetBuilderBounce : ITargetBuilderBounce {
            private TargetInvoker TargetInvoker;

            public FakeTargetBuilderBounce() {
                DescriptionOutput = new StringWriter();
                TargetInvoker = new TargetInvoker(this);
            }

            public ILog Log {
                get { return new Mock<ILog>().Object; }
            }

            public IShellCommandExecutor ShellCommand {
                get { return new Mock<IShellCommandExecutor>().Object; }
            }

            public LogOptions LogOptions {
                get { return new LogOptions(); }
            }

            public ITaskLogFactory LogFactory { get; set; }

            public void Invoke(IBounceCommand command, ITask task) {
                TargetInvoker.Invoke(command, task);
            }

            public ITaskScope TaskScope(ITask task, IBounceCommand command, string targetName) {
                return new Mock<ITaskScope>().Object;
            }

            public TextWriter DescriptionOutput { get; private set; }
        }

        public static bool IsDependentOn(this ITask task, ITask dependency) {
            if (task == dependency) {
                return true;
            } else {
                return task.Dependencies.Select(d => d.Task).Any(t => t.IsDependentOn(dependency));
            }
        }
    }
}