using System;
using System.Diagnostics;
using System.IO;

namespace Bounce.Framework {
    class ShellCommandExecutor : IShellCommandExecutor {
        public void ExecuteAndExpectSuccess(string commandName, string commandArgs) {
            var output = Execute(commandName, commandArgs);

            if (output.ExitCode != 0) {
                throw new BuildException(String.Format("running: {0} {1}\nin: {2}\nexited with {3}", commandName, commandArgs, Directory.GetCurrentDirectory(), output.ExitCode), output.ErrorAndOutput);
            }
        }

        public ProcessOutput Execute(string commandName, string commandArgs) {
            var commandLog = BounceRunner.Bounce.Log.BeginExecutingCommand(commandName, commandArgs);

            var processInfo = new ProcessStartInfo(commandName, commandArgs);
            processInfo.CreateNoWindow = true;
            processInfo.RedirectStandardError = true;
            processInfo.RedirectStandardOutput = true;
            processInfo.UseShellExecute = false;

            var p = new Process { StartInfo = processInfo };

            var output = new CommandOutputReceiver(commandLog);

            p.ErrorDataReceived += output.ErrorDataReceived;
            p.OutputDataReceived += output.OutputDataReceived;
            p.Start();

            p.BeginErrorReadLine();
            p.BeginOutputReadLine();

            p.WaitForExit();

            commandLog.CommandComplete(p.ExitCode);

            return new ProcessOutput {ExitCode = p.ExitCode, Output = output.Output, Error = output.Error, ErrorAndOutput = output.ErrorAndOutput};
        }
    }
}