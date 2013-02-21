using System.Diagnostics;

namespace Bounce.Framework {
    public interface IShellLogger {
        void OutputDataReceived(object sender, DataReceivedEventArgs e);
        void ErrorDataReceived(object sender, DataReceivedEventArgs e);
    }
}