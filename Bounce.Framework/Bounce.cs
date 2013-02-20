using System;

namespace Bounce.Framework {
    public class Bounce {
        public static ILog Log { get; private set; }
        public static IShell Shell { get; internal set; }
        public static Arguments Arguments { get; internal set; }

        public static void SetUp(Arguments arguments) {
            var commandLine = new LogOptionCommandLineParser().ParseCommandLine(arguments);
            Log = new Log(Console.Out, Console.Error, commandLine, new DefaultLogFormatter());
            Shell = new Shell(Log);
            Arguments = arguments;
        }
    }
}