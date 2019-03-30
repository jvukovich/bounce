using System.Diagnostics;

namespace Bounce.Framework
{
    class MultiShellLogger : IShellLogger
    {
        private IShellLogger[] Loggers;

        public MultiShellLogger(params IShellLogger[] loggers)
        {
            Loggers = loggers;
        }

        public void OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            foreach (var logger in Loggers)
            {
                logger.OutputDataReceived(sender, e);
            }
        }

        public void ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            foreach (var logger in Loggers)
            {
                logger.ErrorDataReceived(sender, e);
            }
        }
    }
}