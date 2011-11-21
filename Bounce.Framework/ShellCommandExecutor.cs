using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;

namespace Bounce.Framework {
    public class ShellCommandExecutor : IShellCommandExecutor {
        private Func<ILog> GetLog;

        public ShellCommandExecutor(Func<ILog> getLog) {
            GetLog = getLog;
        }

        public ProcessOutput ExecuteAndExpectSuccess(string commandName, string commandArgs)
        {
            var output = Execute(commandName, commandArgs);
            if (output.ExitCode != 0) {
                throw new CommandExecutionException(String.Format("running: {0} {1}\nin: {2}\nexited with {3}", commandName, commandArgs, Directory.GetCurrentDirectory(), output.ExitCode), output.ErrorAndOutput);
            }
            return output;
        }

        public ProcessOutput Execute(string commandName, string commandArgs) {
            ICommandLog commandLog = GetLog().BeginExecutingCommand(commandName, commandArgs);

            string commandArgumentsForLogging = commandLog.CommandArgumentsForLogging;
            var output = new CommandOutputReceiver(commandLog);

            Process p = CreateProcess(commandName, commandArgumentsForLogging, output);

            try
            {
                RunProcess(p);
            }
            catch (Win32Exception win32Ex)
            {
                if (win32Ex.NativeErrorCode == 2)
                { // executable file not found
                    throw new ShellExecutableNotFoundException(commandName);
                }
                throw;
            }

            commandLog.CommandComplete(p.ExitCode);

            return new ProcessOutput
            {
                ExitCode = p.ExitCode,
                Output = output.Output,
                Error = output.Error,
                ErrorAndOutput = output.ErrorAndOutput
            };
        }

        private void RunProcess(Process p)
        {
            ConsoleCancelEventHandler killOnCtrlC = (sender, cancelEvent) => p.Kill();

            p.Start();

            System.Console.CancelKeyPress += killOnCtrlC;

            try
            {
                p.BeginErrorReadLine();
                p.BeginOutputReadLine();
                p.WaitForExit();
            }
            finally
            {
                System.Console.CancelKeyPress -= killOnCtrlC;
            }
        }

        private Process CreateProcess(string commandName, string commandArgumentsForLogging, CommandOutputReceiver output)
        {
            var processInfo = new ProcessStartInfo(commandName, commandArgumentsForLogging);
            processInfo.CreateNoWindow = true;
            processInfo.RedirectStandardError = true;
            processInfo.RedirectStandardOutput = true;
            processInfo.UseShellExecute = false;
            processInfo.ErrorDialog = false;
            if (string.IsNullOrEmpty(processInfo.EnvironmentVariables["HOME"])) {
                processInfo.EnvironmentVariables["HOME"] = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            }

            var p = new Process { StartInfo = processInfo };

            p.ErrorDataReceived += output.ErrorDataReceived;
            p.OutputDataReceived += output.OutputDataReceived;
            return p;
        }
    }
}