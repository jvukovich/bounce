using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Build.BuildEngine;
using System.IO;

namespace Bounce {
    public class VisualStudioSolution : IIisWebSiteDirectory {
        public IValue<string> SolutionPath;
        public IValue<string> OutputProjectName;
        public IEnumerable<string> DllPaths { get; private set; }
        private TextWriter SynchronizedMsBuildOutput;
        private StringWriter MsBuildOutput;

        public IEnumerable<ITarget> Dependencies {
            get { return new[] {SolutionPath}; }
        }

        public void Build() {
            Console.WriteLine("building solution at: " + SolutionPath.Value);

            ExternalBuild();

            LastBuilt = DateTime.UtcNow;

            var binPath = System.IO.Path.Combine(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(SolutionPath.Value), OutputProjectName.Value), @"bin\Debug");
            DllPaths = Directory.GetFiles(binPath);
        }

        private void ExternalBuild() {
            ProcessStartInfo processInfo = new ProcessStartInfo("msbuild.exe", SolutionPath.Value);
            processInfo.CreateNoWindow = true;
            processInfo.RedirectStandardError = true;
            processInfo.RedirectStandardOutput = true;
            processInfo.UseShellExecute = false;

            Process p = new Process();
            p.StartInfo = processInfo;

            MsBuildOutput = new StringWriter();
            SynchronizedMsBuildOutput = TextWriter.Synchronized(MsBuildOutput);

            p.ErrorDataReceived += MsBuildErrorDataReceived;
            p.OutputDataReceived += MsBuildOutputDataReceived;
            p.Start();

            p.BeginErrorReadLine();
            p.BeginOutputReadLine();

            p.WaitForExit();
            if (p.ExitCode != 0) {
                throw new BuildException("msbiuld exited funny", MsBuildOutput.ToString());
            }
        }

        void MsBuildOutputDataReceived(object sender, DataReceivedEventArgs e) {
            SynchronizedMsBuildOutput.WriteLine(e.Data);
        }

        void MsBuildErrorDataReceived(object sender, DataReceivedEventArgs e) {
            SynchronizedMsBuildOutput.WriteLine(e.Data);
        }

        public void Clean() {
            Console.WriteLine("cleaning solution at: " + SolutionPath.Value);
        }

        public DateTime? LastBuilt { get; set; }

        public string Path {
            get { return SolutionPath.Value; }
        }
    }

    public class BuildException : Exception {
        public readonly string Output;

        public BuildException(string message, string output) : base(message) {
            Output = output;
        }
    }
}