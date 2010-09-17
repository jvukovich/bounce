using System.Diagnostics;
using System.IO;

namespace Bounce.Framework {
    class CommandOutputReceiver {
        private readonly TextWriter synchronizedOutputWriter;
        private readonly TextWriter synchronizedErrorWriter;
        private readonly TextWriter synchronizedCombinedWriter;
        private readonly StringWriter outputWriter;
        private readonly StringWriter errorWriter;
        private readonly StringWriter combinedWriter;

        public CommandOutputReceiver() {
            outputWriter = new StringWriter();
            synchronizedOutputWriter = TextWriter.Synchronized(outputWriter);
            errorWriter = new StringWriter();
            synchronizedErrorWriter = TextWriter.Synchronized(errorWriter);
            combinedWriter = new StringWriter();
            synchronizedCombinedWriter = TextWriter.Synchronized(combinedWriter);
        }

        public void OutputDataReceived(object sender, DataReceivedEventArgs e) {
            synchronizedOutputWriter.WriteLine(e.Data);
            synchronizedCombinedWriter.WriteLine(e.Data);
        }

        public void ErrorDataReceived(object sender, DataReceivedEventArgs e) {
            synchronizedErrorWriter.WriteLine(e.Data);
            synchronizedCombinedWriter.WriteLine(e.Data);
        }

        public string Output {
            get {
                return outputWriter.ToString();
            }
        }

        public string Error {
            get {
                return errorWriter.ToString();
            }
        }

        public string ErrorAndOutput {
            get {
                return combinedWriter.ToString();
            }
        }
    }
}