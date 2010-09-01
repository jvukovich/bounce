using System.Diagnostics;
using System.IO;

namespace Bounce {
    class CommandOutput {
        private readonly TextWriter SynchronizedMsBuildOutput;
        private readonly StringWriter MsBuildOutput;

        public CommandOutput() {
            MsBuildOutput = new StringWriter();
            SynchronizedMsBuildOutput = TextWriter.Synchronized(MsBuildOutput);
        }

        public void MsBuildOutputDataReceived(object sender, DataReceivedEventArgs e) {
            SynchronizedMsBuildOutput.WriteLine(e.Data);
        }

        public void MsBuildErrorDataReceived(object sender, DataReceivedEventArgs e) {
            SynchronizedMsBuildOutput.WriteLine(e.Data);
        }

        public string Output {
            get {
                return MsBuildOutput.ToString();
            }
        }
    }
}