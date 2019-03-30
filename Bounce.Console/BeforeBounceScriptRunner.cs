using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Bounce.Console
{
    [Serializable]
    class BeforeBounceScriptRunner
    {
        public void RunBeforeBounceScript(OptionsAndArguments optionsAndArguments)
        {
            string script = BeforeBounceScript(optionsAndArguments.BounceDirectory);
            if (script != null)
            {
                LogRunningBeforeBounceScript(script);

                var processInfo = new ProcessStartInfo(script);
                processInfo.CreateNoWindow = true;
                processInfo.RedirectStandardError = true;
                processInfo.RedirectStandardOutput = true;
                processInfo.UseShellExecute = false;

                var p = new Process {StartInfo = processInfo};
                var capturer = new RedirectedProcessOutputCapturer();
                p.Start();
                capturer.CaptureOutputForProcess(p);
                p.WaitForExit();

                if (p.ExitCode != 0)
                {
                    throw new BeforeBounceScriptException(script, capturer.CapturedOutput, p.ExitCode);
                }
            }
        }

        private string BeforeBounceScript(string bounceDir)
        {
            return Directory.GetFiles(bounceDir, "beforebounce.*").FirstOrDefault();
        }

        private static void LogRunningBeforeBounceScript(string executable)
        {
            string relativeExecutablePath;
            var currentDirectory = Directory.GetCurrentDirectory();
            if (executable.StartsWith(currentDirectory))
            {
                relativeExecutablePath = executable.Substring(currentDirectory.Length).TrimStart('\\');
            } else
            {
                relativeExecutablePath = executable;
            }
        }
    }

    class RedirectedProcessOutputCapturer
    {
        private StringWriter Stream;

        public void CaptureOutputForProcess(Process p)
        {
            Stream = new StringWriter();
            p.ErrorDataReceived += DataReceived;
            p.OutputDataReceived += DataReceived;
            p.BeginErrorReadLine();
            p.BeginOutputReadLine();
        }

        private void DataReceived(object sender, DataReceivedEventArgs e)
        {
            Stream.WriteLine(e.Data);
        }

        public string CapturedOutput
        {
            get { return Stream.ToString(); }
        }
    }
}