using System.Diagnostics;
using System.IO;

namespace Bounce.Framework {
    class CommandOutputReceiver {
        private readonly TextWriter synchronizedOutputWriter;
        private readonly StringWriter outputWriter;

        public CommandOutputReceiver() {
            outputWriter = new StringWriter();
            synchronizedOutputWriter = TextWriter.Synchronized(outputWriter);
        }

        public void OutputDataReceived(object sender, DataReceivedEventArgs e) {
            synchronizedOutputWriter.WriteLine(e.Data);
        }

        public void ErrorDataReceived(object sender, DataReceivedEventArgs e) {
            synchronizedOutputWriter.WriteLine(e.Data);
        }

        public string Output {
            get {
                return outputWriter.ToString();
            }
        }
    }
}