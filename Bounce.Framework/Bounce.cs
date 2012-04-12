using System;

namespace Bounce.Framework {
    public class Bounce {
        public static IShellCommandExecutor Shell { get; internal set; }

        public static void SetUp() {
            ILog log = new Log(Console.Out, Console.Error, new LogOptions(), new DefaultLogFormatter());
            Shell = new ShellCommandExecutor(() => log);
        }
    }
}