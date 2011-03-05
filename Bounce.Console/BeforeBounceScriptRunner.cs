using System;
using System.Diagnostics;
using System.IO;

namespace Bounce.Console {
    class BeforeBounceScriptRunner {
        public void RunBeforeBounceScript(OptionsAndArguments optionsAndArguments) {
            if (optionsAndArguments.TargetsAssembly.ExecutableType == BounceDirectoryExecutableType.BeforeBounce) {
                LogRunningBeforeBounceScript(optionsAndArguments.TargetsAssembly.Executable);

                var processInfo = new ProcessStartInfo(optionsAndArguments.TargetsAssembly.Executable);
                processInfo.CreateNoWindow = true;
                processInfo.RedirectStandardError = true;
                processInfo.RedirectStandardOutput = true;
                processInfo.UseShellExecute = false;
                
                var p = new Process { StartInfo = processInfo };
                var capturer = new RedirectedProcessOutputCapturer();
                p.Start();
                capturer.CaptureOutputForProcess(p);
                p.WaitForExit();

                if (p.ExitCode != 0) {
                    throw new BeforeBounceScriptException(optionsAndArguments.TargetsAssembly.Executable,
                                                          capturer.CapturedOutput, p.ExitCode);
                }

                optionsAndArguments.TargetsAssembly.Executable = @"Bounce\Targets.dll";
                optionsAndArguments.TargetsAssembly.ExecutableType = BounceDirectoryExecutableType.Targets;
            }
        }

        private static void LogRunningBeforeBounceScript(string executable) {
            string relativeExecutablePath;
            var currentDirectory = Directory.GetCurrentDirectory();
            if (executable.StartsWith(currentDirectory)) {
                relativeExecutablePath = executable.Substring(currentDirectory.Length).TrimStart('\\');
            } else {
                relativeExecutablePath = executable;
            }
            System.Console.WriteLine("running {0}", relativeExecutablePath);
        }
    }

    class RedirectedProcessOutputCapturer {
        private StringWriter Stream;

        public void CaptureOutputForProcess(Process p) {
            Stream = new StringWriter();
            p.ErrorDataReceived += DataReceived;
            p.OutputDataReceived += DataReceived;
            p.BeginErrorReadLine();
            p.BeginOutputReadLine();
        }

        private void DataReceived(object sender, DataReceivedEventArgs e) {
            Stream.WriteLine(e.Data);
        }

        public string CapturedOutput {
            get { return Stream.ToString(); }
        }
    }
}