using System.Collections.Generic;
using System.IO;
using System.Linq;
using Bounce.Framework.Obsolete;
using Moq;

namespace Bounce.Framework.Tests.Obsolete {
    public static class TaskTestExtensions {
        public static void TestBuild(this IObsoleteTask task) {
            TestInvoke(task, new BounceCommandParser().Build);
        }

        public static void TestClean(this IObsoleteTask task) {
            TestInvoke(task, new BounceCommandParser().Clean);
        }

        public static void TestInvoke(this IObsoleteTask task, IBounceCommand command) {
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

            public IEnumerable<IParameter> ParametersGiven { get; set; }

            public void Invoke(IBounceCommand command, IObsoleteTask task) {
                TargetInvoker.Invoke(command, task);
            }

            public ITaskScope TaskScope(IObsoleteTask task, IBounceCommand command, string targetName) {
                return new Mock<ITaskScope>().Object;
            }

            public TextWriter DescriptionOutput { get; private set; }
        }

        public static bool IsDependentOn(this IObsoleteTask task, IObsoleteTask dependency) {
            if (task == dependency) {
                return true;
            } else {
                return task.Dependencies.Select(d => d.Task).Any(t => t.IsDependentOn(dependency));
            }
        }
    }
}