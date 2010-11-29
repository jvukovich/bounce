using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace Bounce.Framework {
    public class ShellCommandExecutor : IShellCommandExecutor {
        public void ExecuteAndExpectSuccess(string commandName, string commandArgs) {
            var output = Execute(commandName, commandArgs);
            if (output.ExitCode != 0) {
                throw new CommandExecutionException(String.Format("running: {0} {1}\nin: {2}\nexited with {3}", commandName, commandArgs, Directory.GetCurrentDirectory(), output.ExitCode), output.ErrorAndOutput);
            }
        }

        public ProcessOutput Execute(string commandName, string commandArgs) {
            var commandLog = BounceRunner.Bounce.Log.BeginExecutingCommand(commandName, commandArgs);

            var processInfo = new ProcessStartInfo(commandName, commandLog.CommandArgumentsForLogging);
            processInfo.CreateNoWindow = true;
            processInfo.RedirectStandardError = true;
            processInfo.RedirectStandardOutput = true;
            processInfo.UseShellExecute = false;
            processInfo.ErrorDialog = true;
            
            var p = new Process { StartInfo = processInfo };

            var output = new CommandOutputReceiver(commandLog);
            
            p.ErrorDataReceived += output.ErrorDataReceived;
            p.OutputDataReceived += output.OutputDataReceived;

            try {
                p.Start();
                
                p.BeginErrorReadLine();
                p.BeginOutputReadLine();
                p.WaitForExit();
                
                commandLog.CommandComplete(p.ExitCode);

                return new ProcessOutput {
                                             ExitCode = p.ExitCode,
                                             Output = output.Output,
                                             Error = output.Error,
                                             ErrorAndOutput = output.ErrorAndOutput
                                         };
            } catch (Win32Exception win32Ex) {
                if (win32Ex.NativeErrorCode == 2) { // executable file not found
                    throw new ShellExecutableNotFoundException(commandName);
                }
                throw;
            }
        }
    }
}