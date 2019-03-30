using System;
using System.Diagnostics;

namespace Bounce.Framework
{
    class ConsoleShellLogger : IShellLogger
    {
        public void OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            Console.Out.WriteLine(e.Data);
        }

        public void ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            Console.Out.WriteLine(e.Data);
        }
    }
}