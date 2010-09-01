using System.Diagnostics;

namespace Bounce {
    class ShellCommandExecutor {
        public void ExternalBuild(string commandName, string commandArgs, string errorMessage) {
            var processInfo = new ProcessStartInfo(commandName, commandArgs);
            processInfo.CreateNoWindow = true;
            processInfo.RedirectStandardError = true;
            processInfo.RedirectStandardOutput = true;
            processInfo.UseShellExecute = false;

            var p = new Process { StartInfo = processInfo };

            var output = new CommandOutput();

            p.ErrorDataReceived += output.MsBuildErrorDataReceived;
            p.OutputDataReceived += output.MsBuildOutputDataReceived;
            p.Start();

            p.BeginErrorReadLine();
            p.BeginOutputReadLine();

            p.WaitForExit();
            if (p.ExitCode != 0) {
                throw new BuildException(errorMessage, output.Output);
            }
        }
    }
}