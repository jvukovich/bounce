using System.Diagnostics;
using System.IO;

namespace Bounce.Framework
{
    class StringShellLogger : IShellLogger
    {
        private readonly TextWriter SynchronizedOutputWriter;
        private readonly TextWriter SynchronizedErrorWriter;
        private readonly TextWriter SynchronizedCombinedWriter;
        private readonly StringWriter OutputWriter;
        private readonly StringWriter ErrorWriter;
        private readonly StringWriter CombinedWriter;

        public StringShellLogger()
        {
            OutputWriter = new StringWriter();
            SynchronizedOutputWriter = TextWriter.Synchronized(OutputWriter);
            ErrorWriter = new StringWriter();
            SynchronizedErrorWriter = TextWriter.Synchronized(ErrorWriter);
            CombinedWriter = new StringWriter();
            SynchronizedCombinedWriter = TextWriter.Synchronized(CombinedWriter);
        }

        public void OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            SynchronizedOutputWriter.WriteLine(e.Data);
            SynchronizedCombinedWriter.WriteLine(e.Data);
        }

        public void ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            SynchronizedErrorWriter.WriteLine(e.Data);
            SynchronizedCombinedWriter.WriteLine(e.Data);
        }

        public string Output
        {
            get { return OutputWriter.ToString(); }
        }

        public string Error
        {
            get { return ErrorWriter.ToString(); }
        }

        public string ErrorAndOutput
        {
            get { return CombinedWriter.ToString(); }
        }
    }
}