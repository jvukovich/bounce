using System;

namespace Bounce.Framework {
    public class LogOptions {
        public LogLevel LogLevel;
        public bool ReportTaskStart = true;
        public bool ReportTaskEnd;
        public bool ReportTargetStart;
        public bool ReportTargetEnd;
        public bool CommandOutput;
        public bool DescribeTasks;
    }
}