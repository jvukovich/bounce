using System.IO;

namespace Bounce.Framework.Obsolete {
    public class LogOptions {
        public LogLevel LogLevel;
        public bool ReportTaskStart = true;
        public bool ReportTaskEnd;
        public bool ReportTargetStart;
        public bool ReportTargetEnd;
        public bool CommandOutput;
        public bool DescribeTasks;
        public TextWriter StdOut = System.Console.Out;
        public TextWriter StdErr = System.Console.Error;
    }
}