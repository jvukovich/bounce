using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;

namespace Bounce.Framework {
    public class Shell : IShell {
        private ILog Log;

        public Shell(ILog log) {
            Log = log;
        }

        public ProcessOutput Exec(string command, IShellLogger logger = null) {
            return Exec("cmd", "/c " + command);
        }

        public ProcessOutput Exec(string commandName, string commandArgs, IShellLogger overridingShellLogger = null) {
            var stringLogger = new StringShellLogger();
            var logger = new MultiShellLogger(overridingShellLogger ?? new ConsoleShellLogger(), stringLogger);

            var exitCode = Execute(commandName, commandArgs, logger);

            if (exitCode != 0) {
                throw new CommandExecutionException(String.Format("running: {0} {1}\nin: {2}\nexited with {3}", commandName, commandArgs, Directory.GetCurrentDirectory(), exitCode), stringLogger.ErrorAndOutput);
            }

            return new ProcessOutput {
                ExitCode = exitCode,
                Error = stringLogger.Error,
                Output = stringLogger.Output,
                ErrorAndOutput = stringLogger.ErrorAndOutput
            };
        }

        public int Execute(string commandName, string commandArgs, IShellLogger logger) {
            Log.Info("in: {0}", Directory.GetCurrentDirectory());
            Log.Info("exec: {0} {1}", commandName, commandArgs);

            Process p = CreateProcess(commandName, commandArgs, logger);

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

            Log.Info("command: {0}, complete with: {1}", commandName, p.ExitCode);

            return p.ExitCode;
        }

        private void RunProcess(Process p)
        {
            ConsoleCancelEventHandler killOnCtrlC = (sender, cancelEvent) => p.Kill();

            p.Start();

            Console.CancelKeyPress += killOnCtrlC;

            try
            {
                p.BeginErrorReadLine();
                p.BeginOutputReadLine();
                p.WaitForExit();
            }
            finally
            {
                Console.CancelKeyPress -= killOnCtrlC;
            }
        }

        private Process CreateProcess(string commandName, string commandArgumentsForLogging, IShellLogger output)
        {
            var processInfo = new ProcessStartInfo(commandName, commandArgumentsForLogging);
            processInfo.CreateNoWindow = true;
            processInfo.RedirectStandardError = true;
            processInfo.RedirectStandardOutput = true;
            processInfo.UseShellExecute = false;
            processInfo.ErrorDialog = false;
            if (string.IsNullOrEmpty(processInfo.EnvironmentVariables["HOME"])) {
                processInfo.EnvironmentVariables["HOME"] = Environment.GetEnvironmentVariable("UserProfile");
            }

            var p = new Process { StartInfo = processInfo };

            p.ErrorDataReceived += output.ErrorDataReceived;
            p.OutputDataReceived += output.OutputDataReceived;
            return p;
        }
    }
}