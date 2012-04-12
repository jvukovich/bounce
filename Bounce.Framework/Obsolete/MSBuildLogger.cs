using System.IO;
using Microsoft.Build.Framework;

namespace Bounce.Framework.Obsolete {
    public class MSBuildLogger : ILogger {
        private TextWriter Output;

        public void Initialize(IEventSource eventSource) {
            eventSource.ErrorRaised += eventSource_ErrorRaised;
            eventSource.WarningRaised += eventSource_WarningRaised;
            Output = System.Console.Out;
        }

        void eventSource_WarningRaised(object sender, BuildWarningEventArgs e) {
            Output.WriteLine("{0}({1},{2}): warning {3}: {4}", e.File, e.LineNumber, e.ColumnNumber, e.Code, e.Message);
        }

        void eventSource_ErrorRaised(object sender, BuildErrorEventArgs e) {
            Output.WriteLine("{0}({1},{2}): error {3}: {4}", e.File, e.LineNumber, e.ColumnNumber, e.Code, e.Message);
        }

        public void Shutdown() {
            Output.Close();
        }

        public LoggerVerbosity Verbosity { get; set; }
        public string Parameters { get; set; }
    }
}