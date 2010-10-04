using System.IO;
using Microsoft.Build.Framework;

namespace Bounce.Framework {
    public class MSBuildLogger : ILogger {
        private TextWriter Output;

        public void Initialize(IEventSource eventSource) {
            eventSource.ErrorRaised += eventSource_ErrorRaised;
//            Output = new StreamWriter(new FileStream("msbuild_output.txt", FileMode.Create));
            Output = System.Console.Out;
        }

        void eventSource_ErrorRaised(object sender, BuildErrorEventArgs e) {
//            Output.WriteLine("code: " + e.Code);
//            Output.WriteLine("colnumber: " + e.ColumnNumber);
//            Output.WriteLine("endcolnumber: " + e.EndColumnNumber);
//            Output.WriteLine("endlinenumber: " + e.EndLineNumber);
//            Output.WriteLine("file: " + e.File);
//            Output.WriteLine("linenumber: " + e.LineNumber);
//            Output.WriteLine("message: " + e.Message);
//            Output.WriteLine("subcat: " + e.Subcategory);
//            Output.WriteLine("timestamp: " + e.Timestamp);

            Output.WriteLine("{0}({1},{2}): error {3}: {4}", e.File, e.LineNumber, e.ColumnNumber, e.Code, e.Message);
        }

        public void Shutdown() {
            Output.Close();
        }

        public LoggerVerbosity Verbosity { get; set; }
        public string Parameters { get; set; }
    }
}